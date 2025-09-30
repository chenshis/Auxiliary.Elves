
USE auxiliary;
DROP TABLE IF EXISTS sys_user;
CREATE TABLE sys_user(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `UserId` VARCHAR(50) NOT NULL COMMENT '编码',
    `UserFeatureCode` VARCHAR(500) NOT NULL COMMENT '特征码',
    `UserName` VARCHAR(50) NOT NULL COMMENT '账号',
    `UserBakckupNumber` VARCHAR(8) NOT NULL COMMENT '备用数字',
    `UserBindAccount` VARCHAR(100) COMMENT '绑定谷歌账号',
    `UserInviteUserName` VARCHAR(100) COMMENT '邀请人账号',
    `IsEnable` BOOLEAN COMMENT '是否启用',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_user';

