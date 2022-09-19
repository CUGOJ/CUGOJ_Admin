<script setup lang="ts">
import { getHosts } from '@/api/sysinfo'
import type { Host } from '@/models/models';
import { NButton, type DataTableColumns } from 'naive-ui';
import type { PropType } from 'vue';
const props = defineProps({
    value: Array as PropType<Array<Host>>,
    withDelete: Boolean,
    withSelect: Boolean
})
const emit = defineEmits<{
    (e: 'hostsUpdate', name: string): void
}>()
const internalInstance = getCurrentInstance()
const internalData = internalInstance?.appContext.config.globalProperties
const cookies = internalData?.$cookies

const user = cookies.get('username')

const createColums = () => {
    var res: DataTableColumns<Host> = [
        {
            title: "主机名",
            key: "name",
        },
        {
            title: "主机IP",
            key: "hostIP",
        },
        {
            title: "登录用户",
            key: "user",
        },];
    if (props.withDelete) {
        res.push({
            title: "操作",
            key: "name",
            render(row) {
                return h(
                    NButton,
                    {
                        size: "small",
                        onClick: () => emit('hostsUpdate', row.name),
                        type: "error",
                        disabled: user !== 'admin'
                    },
                    { default: () => '删除主机' }
                )
            }
        })
    }
    return res
}

const columns: DataTableColumns<Host> = createColums()

</script>
<template>
    <n-data-table :columns="columns" :data="props.value" :bordered="false" :pagination="false"></n-data-table>
</template>