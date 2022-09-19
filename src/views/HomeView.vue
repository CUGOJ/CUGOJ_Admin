<script setup lang="ts">
import { getSysInfo } from '@/api/sysinfo'
import type { SysInfo } from '@/models/models';
const selfSysInfo = ref<SysInfo>({
    id: 0,
    env: "",
    logPath: "",
    tracePath: "",
    mysqlPath: "",
    redisPath: "",
    rabbitmqPath: "",
    neo4jPath: "",
})
const publicSysInfo = ref<SysInfo>({
    id: 0,
    env: "",
    logPath: "",
    tracePath: "",
    mysqlPath: "",
    redisPath: "",
    rabbitmqPath: "",
    neo4jPath: "",
})
const message = useMessage()
const updateSysInfo = async () => {
    await getSysInfo().then(sysInfos => {
        if (sysInfos == null || sysInfos.length != 2) {
            message.error("获取系统信息失败")
        } else {
            console.log(sysInfos)
            sysInfos = sysInfos.sort((a, b) => {
                return a.env == 'public' ? -1 : 1;
            })
            console.log(sysInfos)
            publicSysInfo.value = sysInfos[0]
            selfSysInfo.value = sysInfos[1]
            console.log(selfSysInfo.value)
            console.log(publicSysInfo.value)
        }
    })
}
onBeforeMount(async () => {
    await updateSysInfo()
})

const onDeploy = async (env: String, key: String) => {
    if (key == 'mysql')
        showMysqlDeploy.value = true
    curEnv.value = env.toString()
}
const showMysqlDeploy = ref(false)
const curEnv = ref('')

const closeAllModal = () => {
    showMysqlDeploy.value = false
    updateSysInfo()
}

</script>
<template>
    <n-card title="公共资源池">
        <sys-info :value="publicSysInfo" @deploy="onDeploy"></sys-info>
    </n-card>
    <n-card title="私有资源池">
        <sys-info :value="selfSysInfo" @deploy="onDeploy"></sys-info>
    </n-card>
    <mysql-deploy :show="showMysqlDeploy" :env="curEnv" @close="closeAllModal"></mysql-deploy>
</template>