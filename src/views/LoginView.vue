<script setup lang="ts">
import type { LoginStruct } from '@/models/loginStruct';
import { login } from '@/api/login';
import type { FormInst } from 'naive-ui';

const loginModel: LoginStruct = reactive({
    Username: "",
    Password: "",
})

const rules = {
    Username: [
        { required: true, message: '请输入用户名', trigger: 'blur' },
        { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
    ],
    Password: [
        { required: true, message: '请输入密码', trigger: 'blur' },
        { min: 1, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
    ]
}

const message = useMessage()

const onLogin = () => {
    formRef.value?.validate(errors => {
        if (errors) {
            message.error("参数错误")
        } else {
            login(loginModel.Username, loginModel.Password)
                .then((res) => {
                    if (res)
                        message.success('登录成功')
                    else
                        message.error('登录失败')
                })
                .catch((err) => {
                    console.log(err);
                    message.error('登录失败' + err)
                })
        }
    })

}

const formRef = ref<FormInst | null>(null)

</script>

<template>
    <n-card title="登录">
        <n-form :label-width="160" :model="loginModel" size="large" :rules="rules" ref="formRef">
            <n-form-item label="用户名" path="username">
                <n-input v-model:value="loginModel.Username" placeholder="用户名" />
            </n-form-item>
            <n-form-item label="密码" path="password">
                <n-input v-model:value="loginModel.Password" placeholder="密码" type="password" />
            </n-form-item>
            <n-form-item>
                <n-button @click="onLogin">
                    登录
                </n-button>
            </n-form-item>
        </n-form>
    </n-card>
</template>