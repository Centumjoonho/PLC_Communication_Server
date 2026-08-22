-- Optional sample data for local development.
-- 192.0.2.0/24 is TEST-NET-1 and is not routable on the public Internet.
-- All sample PLC rows are disabled by default to prevent unintended connection attempts.

USE `plc_monitoring`;

INSERT INTO `ro_plc_master`
  (`plc_code`, `plc_name`, `plc_ip`, `plc_port`, `memory_address`, `use_yn`)
VALUES
  ('TEST_PLC_01', 'Sample PLC 01', '192.0.2.10', 502, '0030', 'N'),
  ('TEST_PLC_02', 'Sample PLC 02', '192.0.2.11', 502, '0030', 'N'),
  ('TEST_PLC_03', 'Sample PLC 03', '192.0.2.12', 502, '0030', 'N')
ON DUPLICATE KEY UPDATE
  `plc_name`       = VALUES(`plc_name`),
  `plc_ip`         = VALUES(`plc_ip`),
  `plc_port`       = VALUES(`plc_port`),
  `memory_address` = VALUES(`memory_address`),
  `use_yn`         = VALUES(`use_yn`);
