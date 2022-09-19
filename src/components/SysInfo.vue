<script setup lang ="ts">
import { NButton, type DataTableColumns } from 'naive-ui';
import type { SysInfo } from '@/models/models'
import type { PropType } from 'vue'
import type { Ref } from 'vue'

interface SysInfoLine {
    key: string,
    value: string,
}

const internalInstance = getCurrentInstance()
const internalData = internalInstance?.appContext.config.globalProperties
const cookies = internalData?.$cookies

const user = cookies.get('username')

const columns: DataTableColumns<SysInfoLine> = [
    {
        "title": "资源类型",
        "key": "key",
        "width": 180,
    },
    {
        "title": "资源值",
        "key": "value",
    },
    {
        title: "操作",
        "key": "key",
        "width": 250,
        render(row) {
            return h(
                NButton,
                {
                    size: "small",
                    onClick: () => emit('deploy', props.value ? props.value.env : 'unknown', row.key),
                    type: "primary",
                    ghost: true,
                    disabled: !(props.value != null && (props.value.env == user || (props.value.env === 'public' && user === 'admin')))
                },
                { default: () => row.value == '' ? '部署' : '重新部署' }
            )
        }
    }
]

const emit = defineEmits<{
    (e: 'deploy', env: string, type: string): void
}>()

const props = defineProps({
    value: Object as PropType<SysInfo>,
})

const createData: () => Array<SysInfoLine> = () => {
    if (props.value == null) {
        return []
    }
    var lines: Array<SysInfoLine> = [];
    lines.push({ key: "env", value: props.value.env })
    lines.push({ key: "mysql", value: props.value.mysqlPath })
    lines.push({ key: "redis", value: props.value.redisPath })
    lines.push({ key: "rabbitmq", value: props.value.rabbitmqPath })
    lines.push({ key: "neo4j", value: props.value.neo4jPath })
    return lines
}

const data: Ref<Array<SysInfoLine>> = ref([])

onMounted(() => {
    data.value = createData();
})


</script>

<template>
    <n-data-table :columns="columns" :data="createData()" :bordered="false" :pagination="false"></n-data-table>
</template>