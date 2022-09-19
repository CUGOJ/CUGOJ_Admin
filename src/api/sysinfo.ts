import { get, post } from './api'
import type { SysInfo, Host } from '../models/models'
import type { AxiosResponse } from 'axios'

const getSysInfo = async (): Promise<SysInfo[] | null> => {

    var sysInfoList: Array<SysInfo> | null = null
    await get('/getSysInfo', {})
        .then((res: any) => {
            sysInfoList = res;
        }).catch(err => {
            console.log(err)
        })
    return sysInfoList
}

const getHosts = async (): Promise<Host[] | null> => {
    var hostList: Array<Host> | null = null
    await get('/getHosts', {})
        .then((res: any) => {
            hostList = res;
        }).catch(err => {
            console.log(err)
        })
    return hostList
}
export interface AddHostRequest {
    name: string
    ip: string
    user: string
    password: string
}
const addHost = async (req: AddHostRequest): Promise<Boolean> => {
    var addHostResult = false;
    await post('/addHost', req)
        .then((res: any) => {
            addHostResult = res
        })
        .catch(err => {
            console.log(err)
        })
    return addHostResult;
}


const removeHost = async (req: String): Promise<Boolean> => {
    var removeHostResult = false;
    await post('/removeHost', {
        'name': req
    }).then((res: any) => {
        removeHostResult = res
    }).catch(err => {
        console.log(err)
    })
    return removeHostResult
}

export interface InitMysqlRequest {
    hostName: string
    env: string
    password: string
}

const initMysql = async (req: InitMysqlRequest): Promise<Boolean> => {
    var initMysqlResult = false;
    await post('/initMysql', req).then((res: any) => {
        initMysqlResult = res
    }).catch(err => {
        console.log(err)
    })
    return initMysqlResult
}

const deploy = async (env: String, host: String, key: String): Promise<Boolean> => {
    var deployResult = false;

    return deployResult
}

export interface deployRequest {
    env: string
    hostName: string
    branch: string
}


const deployCore = async (req: deployRequest): Promise<string> => {
    var deployCoreResult = '';
    await post('/deployCore', req).then((res: any) => {
        deployCoreResult = res
    })
    return deployCoreResult
}

const reDeployCore = async (env: string): Promise<string> => {
    var reDeployCoreResult = '';
    await post('/reDeployCore', { env: env }).then((res: any) => {
        reDeployCoreResult = res
    })
    return reDeployCoreResult
}

export {
    getSysInfo,
    getHosts,
    addHost,
    removeHost,
    deploy,
    initMysql,
    deployCore,
    reDeployCore
}