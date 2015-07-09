CREATE TABLE activity_log_person (
    id              SERIAL,
    session_id      UUID,
    full_name       VARCHAR(128) NOT NULL,
    description     VARCHAR(256),
    phone           VARCHAR(32),
    email           VARCHAR(128),

    PRIMARY KEY (id),
    UNIQUE (full_name)
);

COMMENT ON TABLE activity_log_person IS 'Generic person associated with an event';
COMMENT ON COLUMN activity_log_person.session_id IS 'Temporary identifier to link user entering a new activity log with a person';

GRANT ALL PRIVILEGES ON activity_log_person TO "neoanime_belletrix-prod";
GRANT ALL PRIVILEGES ON activity_log_person_id_seq TO "neoanime_belletrix-prod";


CREATE TABLE activity_log (
    id              SERIAL,
    created         TIMESTAMP NOT NULL,
    created_by      INT NOT NULL,
    title           VARCHAR(256) NOT NULL,
    title2          VARCHAR(256),
    title3          VARCHAR(256),
    organizers      VARCHAR(256),
    location        VARCHAR(512),
    types           INT[] NOT NULL,
    start_date      DATE NOT NULL,
    end_date        DATE NOT NULL,
    on_campus       BOOLEAN NOT NULL,
    web_site        VARCHAR(2048),
    notes           VARCHAR(4096),

    PRIMARY KEY (id),
    UNIQUE (title),
    FOREIGN KEY (created_by) REFERENCES users (id)
);

COMMENT ON TABLE activity_log IS 'Event log';
COMMENT ON COLUMN activity_log.types IS 'Conference, community event, etc. values derived from code';
COMMENT ON COLUMN activity_log.start_date IS 'Starting date and time of event';
COMMENT ON COLUMN activity_log.end_date IS 'Ending date and time of event';

GRANT ALL PRIVILEGES ON activity_log TO "neoanime_belletrix-prod";
GRANT ALL PRIVILEGES ON activity_log_id_seq TO "neoanime_belletrix-prod";


CREATE TABLE activity_log_participant (
    event_id            INT NOT NULL,
    person_id           INT NOT NULL,
    participant_type    INT NOT NULL,

    PRIMARY KEY (event_id, person_id),
    FOREIGN KEY (event_id) REFERENCES activity_log (id),
    FOREIGN KEY (person_id) REFERENCES activity_log_person (id)
);

COMMENT ON TABLE activity_log_participant IS 'Association between people and events and their type of participation';
COMMENT ON COLUMN activity_log_participant.participant_type IS 'Denotes attendee or contact, value derived from code';

GRANT ALL PRIVILEGES ON activity_log_participant TO "neoanime_belletrix-prod";
