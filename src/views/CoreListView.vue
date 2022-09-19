<script setup lang="ts">
import type { TableColumns } from 'naive-ui/es/data-table/src/interface';
import type { Core } from '@/models/models';
import { NButton } from 'naive-ui';
import { getCoreList, ping, restart } from '@/api/core';
import { reDeployCore } from '@/api/sysinfo';
const loadCore = async (env: string) => {
    detailEnv.value = env
}

const pingCore = async (env: string) => {
    await ping(env).then(res => {
        if (res == 0) {
            res = null
            message.error('服务器无响应')
        }
        coreList.value.forEach(core => {
            if (core.env == env)
                core.ping = res
        })
    }).catch(err => {
        message.error('ping发生问题:' + err)
        coreList.value.forEach(core => {
            if (core.env == env)
                core.ping = null
        })
    })
}
const message = useMessage();

const reDeploy = async (env: string) => {
    loading.value = true
    reDeployCore(env).then(res => {
        loading.value = false
        message.success('重新部署成功:' + res)
    }).catch(err => {
        loading.value = false
        message.error('重新部署失败' + err)
    })
}
const restartCore = async (env: string) => {
    loading.value = true
    restart(env).then(res => {
        loading.value = false
        message.success('重启成功:' + res)
    }).catch(err => {
        loading.value = false
        message.error('重启失败' + err)
    })
}
const columns: TableColumns<Core> = [
    {
        title: '服务编号',
        key: 'id'
    },
    {
        title: '主机IP',
        key: 'host'
    },
    {
        title: '环境',
        key: 'env'
    },
    {
        title: '操作',
        key: 'id',
        render(row) {
            return [h(
                NButton,
                {
                    size: "small",
                    onClick: () => loadCore(row.env),
                    type: "primary",
                },
                { default: () => '查看详情' }
            ), h(
                NButton,
                {
                    size: "small",
                    onClick: () => pingCore(row.env),
                    style: "margin-left:5px",
                    type: "primary",
                },
                { default: () => row.ping == null ? 'ping' : 'ping:' + row.ping }
            ), h(
                NButton,
                {
                    size: "small",
                    onClick: () => restartCore(row.env),
                    style: "margin-left:5px",
                    type: "warning",
                },
                { default: () => "重启服务" }
            ), h(
                NButton,
                {
                    size: "small",
                    onClick: () => reDeploy(row.env),
                    style: "margin-left:5px",
                    type: "error",
                },
                { default: () => "重新部署" }
            )]
        }
    }
]
const coreList = ref<Array<Core>>([])

const updateCoreList = async () => {
    await getCoreList()
        .then(res => {
            console.log(res)
            coreList.value = res
        }).catch(err => {
            console.log('获取Core列表出错' + err)
        })
}
const internalInstance = getCurrentInstance()
const internalData = internalInstance?.appContext.config.globalProperties
const cookies = internalData?.$cookies

const user = cookies.get('username')

onMounted(() => updateCoreList())

const showModal = ref(false)
const curEnv = ref('')
const onDeploy = (env: string) => {
    curEnv.value = env
    showModal.value = true
}
const onClose = () => {
    showModal.value = false
    updateCoreList()
}
const detailEnv = ref<string | undefined>(undefined)
const loading = ref(false)
</script>
<template>
    <div>
        <n-spin :show="loading">
            <n-button style="margin-top: 5px;margin-left: 10px;" type="primary" :disabled="user===''"
                v-if="coreList.find(c=>c.env==user)==undefined" @click="()=>onDeploy(user)">部署Core</n-button>
            <n-button style="margin-top: 5px;margin-left: 10px;" @click="()=>onDeploy('prod')" type="primary"
                v-if="coreList.find(c=>c.env=='prod')==undefined&&user=='admin'" :disabled="user!=='admin'">部署Core到生产环境
            </n-button>
            <n-data-table :columns="columns" :data="coreList" style="margin-top: 10px;margin-left: 10px;">

            </n-data-table>
            <core-deploy :show-modal="showModal" :env="curEnv" @close="onClose"></core-deploy>
            <template #description>
                正在部署Core服务,可能需要较长时间,请勿关闭弹窗或刷新页面
            </template>
        </n-spin>
        <service-list style="margin-left: 10px;margin-top: 15px;" :env="detailEnv"></service-list>
    </div>
</template>