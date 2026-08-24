-- PLC Communication Server - MariaDB / MySQL initialization
-- Review the database name before running this script.
-- This script is idempotent: existing tables and data are not dropped.

SET NAMES utf8mb4;
SET time_zone = '+09:00';

CREATE DATABASE IF NOT EXISTS `plc_monitoring`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `plc_monitoring`;

-- PLC connection master
CREATE TABLE IF NOT EXISTS `ro_plc_master` (
  `plc_code`       VARCHAR(50)  NOT NULL COMMENT 'Unique PLC code',
  `plc_name`       VARCHAR(100) NOT NULL COMMENT 'Display name',
  `plc_ip`         VARCHAR(45)  NOT NULL COMMENT 'IPv4 or IPv6 address',
  `plc_port`       SMALLINT UNSIGNED NOT NULL DEFAULT 502 COMMENT 'Modbus TCP port',
  `memory_address` VARCHAR(4)   NOT NULL DEFAULT '0030' COMMENT 'Hex holding-register start address',
  `use_yn`         CHAR(1)      NOT NULL DEFAULT 'Y' COMMENT 'Collection enabled: Y/N',
  `created_at`     DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  `updated_at`     DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                                   ON UPDATE CURRENT_TIMESTAMP(3),
  PRIMARY KEY (`plc_code`),
  KEY `idx_ro_plc_master_use_code` (`use_yn`, `plc_code`),
  CONSTRAINT `chk_ro_plc_master_use_yn` CHECK (`use_yn` IN ('Y', 'N'))
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='PLC connection master';

-- Latest state per PLC
CREATE TABLE IF NOT EXISTS `ro_plc_latest` (
  `plc_code`     VARCHAR(50)  NOT NULL,
  `plc_name`     VARCHAR(100) NOT NULL,
  `receive_data` VARCHAR(255) NOT NULL DEFAULT '',
  `status_text`  VARCHAR(20)  NOT NULL DEFAULT 'WAIT',
  `total_seconds` INT UNSIGNED NOT NULL DEFAULT 0,
  `rate`         DOUBLE       NOT NULL DEFAULT 0,
  `receive_time` DATETIME(3)  NULL,
  `client_ip`    VARCHAR(45)  NOT NULL DEFAULT 'SERVER',
  `created_at`   DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  `updated_at`   DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                                 ON UPDATE CURRENT_TIMESTAMP(3),
  PRIMARY KEY (`plc_code`),
  KEY `idx_ro_plc_latest_receive_time` (`receive_time`),
  CONSTRAINT `fk_ro_plc_latest_master`
    FOREIGN KEY (`plc_code`) REFERENCES `ro_plc_master` (`plc_code`)
    ON UPDATE CASCADE
    ON DELETE CASCADE
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Latest PLC status used by the API';

-- Workday operation aggregate
-- work_date is based on the 08:00 to next-day 07:59:59 workday.
CREATE TABLE IF NOT EXISTS `ro_operation_daily` (
  `plc_code`      VARCHAR(50) NOT NULL,
  `work_date`     DATE        NOT NULL,
  `total_seconds` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_00` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_01` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_02` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_03` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_04` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_05` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_06` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_07` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_08` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_09` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_10` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_11` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_12` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_13` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_14` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_15` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_16` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_17` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_18` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_19` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_20` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_21` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_22` INT UNSIGNED NOT NULL DEFAULT 0,
  `hat_23` INT UNSIGNED NOT NULL DEFAULT 0,
  `receive_time`  DATETIME(3) NULL,
  `created_at`    DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  `updated_at`    DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3)
                                  ON UPDATE CURRENT_TIMESTAMP(3),
  PRIMARY KEY (`plc_code`, `work_date`),
  KEY `idx_ro_operation_daily_work_date` (`work_date`),
  CONSTRAINT `fk_ro_operation_daily_master`
    FOREIGN KEY (`plc_code`) REFERENCES `ro_plc_master` (`plc_code`)
    ON UPDATE CASCADE
    ON DELETE CASCADE
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Hourly running seconds and daily total by workday';

-- PLC communication failure history
-- No foreign key is used so that history remains available after a master row is deleted.
CREATE TABLE IF NOT EXISTS `ro_plc_history` (
  `message_id`    CHAR(32)     NOT NULL COMMENT 'Guid without hyphens',
  `plc_code`      VARCHAR(50)  NOT NULL,
  `receive_data`  VARCHAR(255) NOT NULL DEFAULT '',
  `total_seconds` INT UNSIGNED NOT NULL DEFAULT 0,
  `rate`          DOUBLE       NOT NULL DEFAULT 0,
  `raw_frame`     TEXT         NULL,
  `receive_time`  DATETIME(3)  NOT NULL,
  `created_at`    DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
  PRIMARY KEY (`message_id`),
  KEY `idx_ro_plc_history_code_time` (`plc_code`, `receive_time`),
  KEY `idx_ro_plc_history_receive_time` (`receive_time`)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='PLC communication failure history';
