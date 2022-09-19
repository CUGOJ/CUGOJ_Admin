export interface Core {
    id: number;
    host: string;
    env: string;
    port: number;
    logPath: string;
    tracePath: string;
    mysqlPath: string;
    redisPath: string;
    neo4jPath: string;
    rabbitmqPath: string;
    status: number;
    ping: number | null;
}

export interface Auth {
    id: number;
    userId: number;
    coreId: number;
    status: number;
}

export interface SysInfo {
    id: number;
    env: string;
    logPath: string;
    tracePath: string;
    mysqlPath: string;
    redisPath: string;
    neo4jPath: string;
    rabbitmqPath: string;
}

export interface Host {
    id: number;
    name: string;
    hostIP: string;
    user: string;
    password: string;
    publicKey: string;
}

export enum ServiceTypeEnum {
    Core = 1,
    Base = 2,
    File = 3,
    Authentication = 4,
    Gateway = 5,
    Judger = 6,
    Sandbox = 7
}


export interface ServiceBaseInfo {
    rabbitMQAddress: string;
    redisAddress: string;
    mysqlAddress: string;
    logAddress: string;
    traceAddress: string;
    env: string;
    registerTime: number;
    serviceType: ServiceTypeEnum;
    serviceIP: string;
    neo4jAddress: string;
    serviceID: string;
    servicePort: string;
}
