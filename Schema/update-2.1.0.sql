CREATE TABLE exceptions (
Id                  BIGSERIAL NOT NULL,
GUID                UUID NOT NULL,
ApplicationName     VARCHAR(50) NOT NULL,
MachineName         VARCHAR(50) NOT NULL,
CreationDate        TIMESTAMP NOT NULL,
Type                VARCHAR(100) NOT NULL,
IsProtected         BOOLEAN NOT NULL DEFAULT FALSE,
Host                VARCHAR(100) NULL,
Url                 VARCHAR(500) NULL,
HTTPMethod          VARCHAR(10) NULL,
IPAddress           VARCHAR(40) NULL,
Source              VARCHAR(100) NULL,
Message             VARCHAR(1000) NULL,
Detail              TEXT NULL,	
StatusCode          INT NULL,
SQL                 TEXT NULL,
DeletionDate        TIMESTAMP NULL,
FullJson            TEXT NULL,
ErrorHash           INT NULL,
DuplicateCount      INT NOT NULL DEFAULT 1,

PRIMARY KEY (Id)
);

COMMENT ON TABLE exceptions IS 'SQL store for StackExchange.Exceptional error handler';

CREATE INDEX IX_Exceptions_GUID_ApplicationName_DeletionDate_CreationDate ON exceptions (GUID, ApplicationName, DeletionDate, CreationDate DESC);
CREATE INDEX IX_Exceptions_ErrorHash_AppName_CreationDate_DelDate ON exceptions (ErrorHash, ApplicationName, CreationDate DESC, DeletionDate);

GRANT ALL PRIVILEGES ON exceptions TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON exceptions_id_seq TO neoanime_abroadadvisor;

CREATE LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_student(INT) RETURNS VOID AS $delete_student$
    DECLARE
        p_student_id ALIAS FOR $1;
    BEGIN
        RAISE NOTICE 'Deleting student ID %', p_student_id;

        IF NOT EXISTS (SELECT 1 FROM students WHERE id = p_student_id) THEN
            RAISE EXCEPTION 'Student ID % not found', p_student_id;
        END IF;

        DELETE FROM event_log WHERE student_id = p_student_id;
        DELETE FROM matriculation WHERE student_id = p_student_id;
        DELETE FROM student_desired_languages WHERE student_id = p_student_id;
        DELETE FROM student_fluent_languages WHERE student_id = p_student_id;
        DELETE FROM student_notes WHERE student_id = p_student_id;
        DELETE FROM student_study_abroad_wishlist WHERE student_id = p_student_id;
        DELETE FROM study_abroad WHERE student_id = p_student_id;
        DELETE FROM students WHERE id = p_student_id;
    END;
$delete_student$ LANGUAGE plpgsql;
