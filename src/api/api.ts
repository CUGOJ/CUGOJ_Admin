import axios from 'axios'


axios.interceptors.request.use(
    config => {
        return config
    }
)

axios.interceptors.response.use(
    response => {
        return response
    }
)

axios.defaults.baseURL = 'https://localhost:7052';

export function get(url: string, params: {} | undefined) {
    return new Promise((resolve, reject) => {
        axios.get(url, { params: params, withCredentials: true }).then(res => {
            resolve(res.data)
        }).catch(err => {
            reject(err)
        })
    })
}

export function post(url: string, params: {} | undefined) {
    return new Promise((resolve, reject) => {
        axios.post(url, params, { withCredentials: true }).then(res => {
            resolve(res.data)
        }).catch(err => {
            reject(err)
        })
    })
}