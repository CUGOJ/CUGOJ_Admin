<script setup lang="ts">

import type { Host } from '@/models/models';
import { getHosts } from '@/api/sysinfo';
import type { InitMysqlRequest } from '@/api/sysinfo';
import type { FormInst } from 'naive-ui';
import { initMysql } from '@/api/sysinfo';
const hosts = ref<Host[]>();
onMounted(() => updateHosts())
const message = useMessage();

const updateHosts = async () => {
    await getHosts().then(res => {
        if (res == undefined) {
            message.error('获取主机信息失败')
            return
        }
        hosts.value = res;
    })
}
const loading = ref(false)
const props = defineProps({
    showModal: Boolean,
    env: String
})

const createOptions = () => {
    var res: Array<{ label: string, value: string }> = []
    if (hosts.value == undefined) {
        return res
    }
    for (var i = 0; i < hosts.value.length; i++) {
        res.push({
            label: hosts.value[i].name,
            value: hosts.value[i].name
        })
    }
    return res
}

const rules = {
    hostName: {
        required: true, message: '请选择主机', trigger: 'blur',
    },
    password: {
        required: true, message: '请输入密码', trigger: 'blur',
    }
}
const initMysqlRequest: InitMysqlRequest = reactive({
    hostName: '',
    env: '',
    password: '',
})
const emit = defineEmits<{
    (e: 'close'): void
}>()
const onInitMysql = () => {
    loading.value = true
    if (props.env == undefined) {
        message.error('未知的环境,请先登录')
        loading.value = false
        return
    }
    initMysqlRequest.env = props.env
    formRef.value?.validate(async errors => {
        if (errors) {
            loading.value = false
            return
        }
        await initMysql(initMysqlRequest).then(res => {
            message.success('初始化成功')
            loading.value = false
            emit('close')
        }).catch(err => {
            message.error('初始化失败:' + err)
            loading.value = false
            return
        })
    })
}

const formRef = ref<FormInst | null>(null)
</script>
<template>
    <n-modal v-model:show="showModal">
        <n-spin :show="loading">
            <n-card style="width:600px" title="部署Mysql" size="huge" role="dialog">
                <n-form ref="formRef" :model="initMysqlRequest" :rules="rules">
                    <n-form-item label="主机名" path="name">
                        <n-select v-model:value="initMysqlRequest.hostName" :options="createOptions()"
                            placeholder="主机名(仅用作标识)" />
                    </n-form-item>
                    <n-form-item label="登录密码" path="password">
                        <n-input v-model:value="initMysqlRequest.password" placeholder="登录密码" />
                    </n-form-item>
                    <n-form-item>
                        <n-space justify="space-between">
                            <n-button type="primary" @click="onInitMysql">部署Mysql</n-button>
                            <n-button type="error" @click="()=>emit('close')">取消</n-button>
                        </n-space>
                    </n-form-item>
                </n-form>
            </n-card>
            <template #description>
                正在部署Mysql,可能需要较长时间,请勿关闭弹窗或刷新页面
            </template>
        </n-spin>
    </n-modal>
</template>