CREATE TABLE location (
id              SERIAL,
country_id      INT NOT NULL,
address         VARCHAR(128),
address2        VARCHAR(128),
locality        VARCHAR(128),
region          VARCHAR(128),
postal_code     VARCHAR(32),

PRIMARY KEY (id),
FOREIGN KEY (country_id) REFERENCES countries (id)
);

COMMENT ON TABLE location IS 'Address information for a general location type';
COMMENT ON COLUMN location.locality IS 'The locality. For example, Greensboro';
COMMENT ON COLUMN location.region IS 'The region. For example, North Carolina';

GRANT ALL PRIVILEGES ON location TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON location_id_seq TO neoanime_abroadadvisor;


CREATE TABLE event_log_person (
id              SERIAL,
full_name       VARCHAR(128) NOT NULL,
description     VARCHAR(256),
phone           VARCHAR(32),
email           VARCHAR(128)
);


CREATE TABLE event_log (
id              SERIAL,
title           VARCHAR(256) NOT NULL,
title2          VARCHAR(256),
title3          VARCHAR(256),
location_id     INT NOT NULL,
start_date      TIMESTAMP NOT NULL,
end_date        TIMESTAMP,
on_campus       BOOLEAN NOT NULL,
web_site        VARCHAR(2048),
notes           VARCHAR(4096),

PRIMARY KEY (id),
FOREIGN KEY (location_id) REFERENCES location (id)
);

COMMENT ON TABLE events IS 'CGS event log';
COMMENT ON COLUMN events.start_date IS 'Starting date and time of event';
COMMENT ON COLUMN events.end_date IS 'Ending date and time if the event was a range of days or NULL if it was a one-time event';
COMMENT ON COLUMN events.tags IS 'Comma-delimited list of event tags';

GRANT ALL PRIVILEGES ON events TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON events_id_seq TO neoanime_abroadadvisor;


CREATE TABLE event_log_types (
event_id        INT NOT NULL,
type_name       VARCHAR(256) NOT NULL,

PRIMARY KEY (event_id, type_name),
FOREIGN KEY (event_id) REFERENCES event_log (id)
);


CREATE TABLE event_log_attendees (
event_id        INT NOT NULL,
person_id       INT NOT NULL,
);


CREATE TABLE event_log_contacts (
event_id        INT NOT NULL,
person_id       INT NOT NULL,
);
