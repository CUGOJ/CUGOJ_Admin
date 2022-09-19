import type { Core, ServiceBaseInfo } from '@/models/models';
import { get, post } from './api'

const getCoreList = async (): Promise<Array<Core>> => {
    var coreList: Array<Core> = []
    await get('/getCoreList', {})
        .then((res: any) => {
            coreList = res
        }).catch(err => {
            console.log(err)
        })
    return coreList
}

const ping = async (env: string): Promise<number | null> => {
    var ping = null
    await get('/ping', { env: env })
        .then((res: any) => {
            ping = res
        }).catch(err => {
            console.log(err)
        })
    return ping
}

const getAllServices = async (env: string): Promise<Array<ServiceBaseInfo>> => {
    var serviceList: Array<ServiceBaseInfo> = []
    await get('/getAllServices', { env: env })
        .then((res: any) => {
            serviceList = res
        }).catch(err => {
            console.log(err)
        })
    return serviceList
}

const restart = async (env: string): Promise<string> => {
    var restartResult = ''

    await post('/restart', { env: env })
        .then((res: any) => {
            restartResult = res
        }).catch(err => {
            console.log(err)
            restartResult = err
        })

    return restartResult
}
export enum SetupServiceTypeEnum {
    Trace = 1,
    Log = 2,
    Mysql = 3,
    Redis = 4,
    RabbitMQ = 5,
    Neo4j = 6
}

const getType = (src: number) => {
    switch (src) {
        case 1: return SetupServiceTypeEnum.Trace
        case 2: return SetupServiceTypeEnum.Log
        case 3: return SetupServiceTypeEnum.Mysql
        case 4: return SetupServiceTypeEnum.Redis
        case 5: return SetupServiceTypeEnum.RabbitMQ
        case 6: return SetupServiceTypeEnum.Neo4j
    }
}

const updateServiceSetup = async (env: string, serviceID: string, setupType: number, value: string) => {
    var updateServiceSetupResult = ''
    await post('/updateServiceSetup', {
        env: env,
        serviceID: serviceID,
        setupType: getType(setupType),
        value: value
    }).then((res: any) => {
        updateServiceSetupResult = res
    }).catch(err => {
        console.log(err)
        updateServiceSetupResult = err
    })
    return updateServiceSetupResult
}

const getConnectionString = async (env: string, serviceID: string) => {
    var connectionString = ''
    await get('/getConnectionString', { env: env, serviceID: serviceID })
        .then((res: any) => {
            connectionString = res
        }).catch(err => {
            console.log(err)
        })
    return connectionString
}

const registerNewService = async (env: string, serviceType: number) => {
    var registerNewServiceResponse = ''
    await post('/registerNewService', { env: env, serviceType: serviceType })
        .then((res: any) => {
            registerNewServiceResponse = res
        }).catch(err => {
            console.log(err)
        })
    return registerNewServiceResponse
}

export {
    getCoreList,
    ping,
    getAllServices,
    restart,
    updateServiceSetup,
    getConnectionString,
    registerNewService
}