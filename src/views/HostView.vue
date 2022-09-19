<script setup lang="ts">
import { getHosts, addHost, removeHost } from '@/api/sysinfo';
import type { Host } from '@/models/models';
import type { FormInst } from 'naive-ui';
import type { AddHostRequest } from '@/api/sysinfo'

const rules = {
    name: {
        required: true,
        message: '请输入主机名(仅用作标识)',
        trigger: 'blur',
        min: 1
    },
    ip: {
        required: true, message: '请输入主机IP', trigger: 'blur'
    },
    user: {
        required: true,
        message: '请输入登录用户',
        trigger: 'blur',
        min: 1
    },
    password: {
        required: true,
        message: '请输入登录密码',
        trigger: 'blur',
        min: 1
    }
}

const message = useMessage();
const hosts = ref<Host[]>();
onMounted(() => updateHosts())

const updateHosts = async () => {
    await getHosts().then(res => {
        if (res == undefined) {
            message.error('获取主机信息失败')
            return
        }
        hosts.value = res;
    })
}

const internalInstance = getCurrentInstance()
const internalData = internalInstance?.appContext.config.globalProperties
const cookies = internalData?.$cookies

const user = cookies.get('username')
const showModal = ref(false)
const addHostRequest: AddHostRequest = reactive({
    name: '',
    ip: '',
    user: '',
    password: ''
})
const formRef = ref<FormInst | null>(null)

const onAddHost = async () => {
    await formRef.value?.validate(async errors => {
        if (errors) {
            message.error("参数错误")
        } else {
            loading.value = true
            await addHost(addHostRequest)
                .then(res => {
                    loading.value = false
                    if (res == true) {
                        message.success('添加成功')
                        showModal.value = false
                        updateHosts()
                    } else {
                        message.error('添加失败,请检查主机信息')
                    }
                })
        }
    })
}

const onRemoveHost = async (name: String) => {
    await removeHost(name)
        .then(res => {
            if (res == true) {
                message.success('删除成功')
                updateHosts()
            } else {
                message.error('删除失败')
            }
        })
}
const loading = ref(false)

</script>
<template>
    <n-button type="primary" v-if="user=='admin'" @click="()=>{showModal=true}" style="margin-bottom: 10px;">接入新主机
    </n-button>
    <host-list :value="hosts" @hosts-update="onRemoveHost" withDelete></host-list>
    <n-modal v-model:show="showModal">
        <n-spin :show="loading">
            <n-card style="width:600px" title="接入新主机" size="huge" role="dialog">
                <n-form ref="formRef" :model="addHostRequest" :rules="rules">
                    <n-form-item label="主机名" path="name">
                        <n-input v-model:value="addHostRequest.name" placeholder="主机名(仅用作标识)" />
                    </n-form-item>
                    <n-form-item label="主机IP" path="ip">
                        <n-input v-model:value="addHostRequest.ip" placeholder="主机IP" />
                    </n-form-item>
                    <n-form-item label="登录用户" path="user">
                        <n-input v-model:value="addHostRequest.user" placeholder="登录用户" />
                    </n-form-item>
                    <n-form-item label="登录密码" path="password">
                        <n-input v-model:value="addHostRequest.password" placeholder="登录密码" />
                    </n-form-item>
                    <n-form-item>
                        <n-button @click="onAddHost">新增主机</n-button>
                    </n-form-item>
                </n-form>
            </n-card>
            <template #description>
                正在添加主机,可能需要较长时间,请勿关闭弹窗或刷新页面
            </template>
        </n-spin>
    </n-modal>
</template>