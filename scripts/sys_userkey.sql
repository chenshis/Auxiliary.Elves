
USE auxiliary;
DROP TABLE IF EXISTS sys_userkey;
CREATE TABLE sys_userkey(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `UserId` VARCHAR(50) NOT NULL COMMENT '账号唯一编码',
    `UserkeyId` VARCHAR(50) NOT NULL COMMENT '卡密唯一编码',
    `UserKey` VARCHAR(50) NOT NULL COMMENT '卡密',
    `UserKeyIp` VARCHAR(200) COMMENT '卡密绑定IP',
    `UserKeyBindAccount` VARCHAR(100) COMMENT '绑定谷歌账号',
    `UserkeyLastDate` DATETIME COMMENT '最后登录日期',
    `IsOnline` BOOLEAN COMMENT '是否在线',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_userkey';

