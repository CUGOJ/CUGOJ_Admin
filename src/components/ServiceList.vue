<script setup lang="ts">
import { getHosts } from '@/api/sysinfo'
import { ServiceTypeEnum, type ServiceBaseInfo } from '@/models/models';
import { NButton, type DataTableColumns } from 'naive-ui';
import type { PropType } from 'vue';
import type { SetupServiceTypeEnum } from '@/api/core'
import { getAllServices, updateServiceSetup, registerNewService, getConnectionString } from '@/api/core'
const props = defineProps({
    env: String,
})
const internalInstance = getCurrentInstance()
const internalData = internalInstance?.appContext.config.globalProperties
const cookies = internalData?.$cookies

const user = cookies.get('username')

const message = useMessage()

watch(props, async (newVal, oldVal) => {
    // console.log(newVal)
    getData()
})

const getData = async () => {
    if (props.env == null || props.env == '')
        data.value = []
    else {
        await getAllServices(props.env)
            .then(res => {
                data.value = res
            }).catch(err => {
                message.error('获取数据出错:' + err)
                data.value = []
            })
    }
}

const data = ref<Array<ServiceBaseInfo>>([])

const createColums = () => {
    var res: DataTableColumns<ServiceBaseInfo> = [
        {
            title: "ServiceID",
            key: "serviceID",
        },
        {
            title: "服务IP",
            key: "serviceIP",
        },
        {
            title: "最后一次注册的端口",
            key: "servicePort",
        },
        {
            title: "注册时间",
            key: "registerTime",
            render(row) {
                return h(
                    'span',
                    {
                        size: "small",
                    },
                    {
                        default: () => {
                            if (row.registerTime == 0)
                                return '服务尚未连接'
                            return new Date(row.registerTime).toISOString()
                        }
                    }
                )
            }
        },
        {
            title: "服务类型",
            key: "serviceType",
            render(row) {
                return h(
                    'span',
                    {
                        size: "small",
                    },
                    {
                        default: () => {
                            switch (row.serviceType) {
                                case ServiceTypeEnum.Authentication:
                                    return 'Authentication'
                                case ServiceTypeEnum.Core:
                                    return 'Core'
                                case ServiceTypeEnum.Gateway:
                                    return 'Gateway'
                                case ServiceTypeEnum.Base:
                                    return 'Base'
                                case ServiceTypeEnum.File:
                                    return 'File'
                                case ServiceTypeEnum.Judger:
                                    return 'Judger'
                                case ServiceTypeEnum.Sandbox:
                                    return 'Sandbox'
                            }
                            return 'Unknown'
                        }
                    }
                )
            }
        },
        {
            title: "操作",
            key: "serviceID",
            render(row) {
                return [h(
                    NButton,
                    {
                        size: "small",
                        type: "primary",
                        onClick: () => {
                            loadService(row.serviceID)
                        },
                        disabled: user !== 'admin' && user !== row.env
                    },
                    {
                        default: () => "查看配置"
                    }
                )]
            }
        }
    ];
    return res
}

const loadService = (serviceID: string) => {
    data.value.forEach(element => {
        if (element.serviceID == serviceID) {
            service.env = element.env
            service.serviceID = element.serviceID
            service.serviceIP = element.serviceIP
            service.servicePort = element.servicePort
            service.serviceType = element.serviceType
            service.registerTime = element.registerTime
            service.mysqlAddress = element.mysqlAddress
            service.redisAddress = element.redisAddress
            service.rabbitMQAddress = element.rabbitMQAddress
            service.neo4jAddress = element.neo4jAddress
            service.logAddress = element.logAddress
            service.traceAddress = element.traceAddress
            showDetail.value = true
            console.log(element)
            if (element.registerTime == 0) {
                getConnectionString(props.env!, element.serviceID)
                    .then(res => {
                        console.log(res)
                        connectionString.value = res
                    }).catch(err => {
                        message.error('获取连接串失败')
                        connectionString.value = undefined
                    })
            } else {
                connectionString.value = undefined
            }
        }
    });
}

const columns: DataTableColumns<ServiceBaseInfo> = createColums()
const service = reactive<ServiceBaseInfo>({
    rabbitMQAddress: '',
    redisAddress: '',
    mysqlAddress: '',
    logAddress: '',
    traceAddress: '',
    env: '',
    registerTime: 0,
    serviceType: ServiceTypeEnum.Base,
    serviceIP: '',
    neo4jAddress: '',
    serviceID: '',
    servicePort: '',

})
const showDetail = ref(false)
const loading = ref(false)
const onUpdateSetup = (setupType: number, value: string) => {
    loading.value = true
    updateServiceSetup(service.env, service.serviceID, setupType, value)
        .then(res => {
            loading.value = false
            message.success('更新配置成功' + res)
        }).catch(err => {
            loading.value = false
            message.error('更新配置失败' + err)
        })
}
const onRegisterNewService = () => {
    loading.value = true
    if (props.env == undefined) {
        message.error('未知的环境')
        return
    }
    registerNewService(props.env, 2)
        .then(res => {
            loading.value = false
            message.success('注册成功')
            getData()
        }).catch(err => {
            loading.value = false
            message.error('注册新服务失败' + err)
        })
}
const connectionString = ref<string | undefined>(undefined)
</script>
<template>
    <div style="margin-left:20px;margin-top:20px">
        <div v-if="env!=null">
            <n-space>
                <n-h1>{{env}} </n-h1>
                <n-button type="primary" @click="onRegisterNewService">注册新服务</n-button>
            </n-space>
        </div>
        <n-data-table :columns="columns" :data="data"></n-data-table>
        <n-modal v-model:show="showDetail" :title="service?.serviceID">
            <n-spin :show="loading">
                <n-card style="width:600px" title="服务配置详情" size="huge" role="dialog">
                    <n-space vertical>
                        <div>
                            服务:{{service?.serviceID}}
                        </div>
                        <div v-if="connectionString!=undefined">
                            连接串:{{connectionString}}
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.logAddress" placeholder="日志中心" />
                                <n-button type="primary" @click="()=>{
                                onUpdateSetup(2,service.logAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.traceAddress" placeholder="追踪中心" />
                                <n-button type="primary" @click="()=>{onUpdateSetup(1,service.traceAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.mysqlAddress" placeholder="数据库连接串" />
                                <n-button type="primary" @click="()=>{onUpdateSetup(3,service.mysqlAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.redisAddress" placeholder="redis地址" />
                                <n-button type="primary" @click="()=>{onUpdateSetup(4,service.redisAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.rabbitMQAddress" placeholder="消息队列地址" />
                                <n-button type="primary" @click="()=>{onUpdateSetup(5,service.rabbitMQAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <div>
                            <n-input-group>
                                <n-input v-model:value="service.neo4jAddress" placeholder="图数据库地址" />
                                <n-button type="primary" @click="()=>{onUpdateSetup(6,service.neo4jAddress)}">更新配置
                                </n-button>
                            </n-input-group>
                        </div>
                        <n-space>
                            <n-button type="primary" @click="showDetail=false">关闭</n-button>
                        </n-space>
                    </n-space>
                </n-card>
                <template #description>
                    正在部署更新服务配置,可能需要较长时间,请勿关闭弹窗或刷新页面
                </template>
            </n-spin>
        </n-modal>
    </div>
</template>