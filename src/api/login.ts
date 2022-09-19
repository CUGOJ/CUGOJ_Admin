import { get, post } from './api'
const login = (username: string, password: string) => {
    return new Promise((resolve, reject) => {
        post('/login', { username: username, password: password }).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

const logout = () => {
    return new Promise((resolve, reject) => {
        post('/logout', {}).then((res: any) => {
            resolve(res)
        }).catch((err: any) => {
            reject(err)
        })
    })
}

export {
    login,
    logout
}