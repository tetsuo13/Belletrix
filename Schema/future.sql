CREATE TABLE advising (
id              SERIAL,
student_id      INTEGER NOT NULL,
country_id      INTEGER,
language_id     INTEGER,
pell_grant      BOOLEAN,

PRIMARY KEY (id)
);

GRANT ALL PRIVILEGES ON advising TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON advising_id_seq TO neoanime_abroadadvisor;
