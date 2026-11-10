-- BaliseManager - sample data for local development
-- Prerequisite: run the EF Core migrations first so that Beacons and BeaconEvents exist.
-- Safe to execute more than once: existing beacon codes and matching events are not duplicated.

PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

-- -----------------------------------------------------------------------------
-- Beacons
-- Status values must match the BeaconStatus enum:
-- Unknown, Active, Inactive, Maintenance
-- -----------------------------------------------------------------------------

INSERT OR IGNORE INTO Beacons
    (Code, Name, Equipment, Site, Status, BatteryLevel,
     LastCommunicationUtc, CreatedAtUtc, UpdatedAtUtc)
VALUES
    ('BCN-001', 'Workshop Beacon',       'Conveyor 1',       'Nantes',                 'Active',      96, '2026-11-10 17:42:00', '2026-11-03 09:00:00', NULL),
    ('BCN-002', 'Storage Beacon',        'Cooling Unit 2',  'Carquefou',              'Active',      17, '2026-11-10 17:38:00', '2026-11-02 10:15:00', NULL),
    ('BCN-003', 'Loading Dock Beacon',   'Dock Door 3',     'Saint-Herblain',         'Inactive',     0, '2026-11-10 08:20:00', '2026-10-28 14:30:00', '2026-11-10 08:25:00'),
    ('BCN-004', 'Maintenance Beacon',    'Packaging Line 2','Reze',                  'Maintenance', 62, '2026-11-10 16:50:00', '2026-10-20 08:00:00', '2026-11-09 11:00:00'),
    ('BCN-005', 'Assembly Beacon',       'Assembly Line 1', 'Sainte-Luce-sur-Loire',  'Active',      78, '2026-11-10 17:35:00', '2026-11-04 07:45:00', NULL),
    ('BCN-006', 'Outdoor Beacon',        'North Gate',      'Coueron',                'Active',      42, '2026-11-10 17:30:00', '2026-10-15 12:00:00', NULL),
    ('BCN-007', 'Server Room Beacon',    'Rack A12',        'Nantes',                 'Active',     100, '2026-11-10 17:44:00', '2026-11-05 15:20:00', NULL);

-- -----------------------------------------------------------------------------
-- Beacon events
-- EventType values must match the BeaconEventType enum:
-- LowBattery, MotionDetected, CommunicationLost, Maintenance, Reconnected
-- -----------------------------------------------------------------------------

-- BCN-001: normal activity
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'MotionDetected', 'Movement detected near Conveyor 1.', '2026-11-10 16:05:00', 97, 1
FROM Beacons b
WHERE b.Code = 'BCN-001'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'MotionDetected'
        AND e.OccurredAtUtc = '2026-11-10 16:05:00'
  );

-- BCN-002: low battery warning
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'LowBattery', 'Battery level is below the 20% warning threshold.', '2026-11-10 15:20:00', 19, 0
FROM Beacons b
WHERE b.Code = 'BCN-002'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'LowBattery'
        AND e.OccurredAtUtc = '2026-11-10 15:20:00'
  );

INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'LowBattery', 'Battery level has dropped to 17%.', '2026-11-10 17:38:00', 17, 0
FROM Beacons b
WHERE b.Code = 'BCN-002'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'LowBattery'
        AND e.OccurredAtUtc = '2026-11-10 17:38:00'
  );

-- BCN-003: communication loss and recovery
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'CommunicationLost', 'No communication received from Dock Door 3.', '2026-11-10 08:25:00', 12, 1
FROM Beacons b
WHERE b.Code = 'BCN-003'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'CommunicationLost'
        AND e.OccurredAtUtc = '2026-11-10 08:25:00'
  );

INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'LowBattery', 'Battery level reached 0%.', '2026-11-10 08:30:00', 0, 1
FROM Beacons b
WHERE b.Code = 'BCN-003'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'LowBattery'
        AND e.OccurredAtUtc = '2026-11-10 08:30:00'
  );

-- BCN-004: maintenance
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'Maintenance', 'Scheduled maintenance started for Packaging Line 2.', '2026-11-09 11:00:00', 64, 1
FROM Beacons b
WHERE b.Code = 'BCN-004'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'Maintenance'
        AND e.OccurredAtUtc = '2026-11-09 11:00:00'
  );

-- BCN-005: normal activity
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'MotionDetected', 'Movement detected on Assembly Line 1.', '2026-11-10 14:12:00', 79, 1
FROM Beacons b
WHERE b.Code = 'BCN-005'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'MotionDetected'
        AND e.OccurredAtUtc = '2026-11-10 14:12:00'
  );

-- BCN-006: low battery warning
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'LowBattery', 'Outdoor Beacon battery level is at 42%.', '2026-11-10 17:30:00', 42, 0
FROM Beacons b
WHERE b.Code = 'BCN-006'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'LowBattery'
        AND e.OccurredAtUtc = '2026-11-10 17:30:00'
  );

-- BCN-007: successful reconnection
INSERT INTO BeaconEvents (BeaconId, EventType, Message, OccurredAtUtc, BatteryLevel, IsRead)
SELECT b.Id, 'Reconnected', 'Server Room Beacon connected successfully.', '2026-11-10 09:10:00', 100, 1
FROM Beacons b
WHERE b.Code = 'BCN-007'
  AND NOT EXISTS (
      SELECT 1 FROM BeaconEvents e
      WHERE e.BeaconId = b.Id AND e.EventType = 'Reconnected'
        AND e.OccurredAtUtc = '2026-11-10 09:10:00'
  );

COMMIT;

-- Quick verification queries (run separately if desired):
-- SELECT Id, Code, Name, Status, BatteryLevel FROM Beacons ORDER BY Code;
-- SELECT e.Id, b.Code, e.EventType, e.Message, e.OccurredAtUtc, e.IsRead
-- FROM BeaconEvents e JOIN Beacons b ON b.Id = e.BeaconId
-- ORDER BY e.OccurredAtUtc DESC;
