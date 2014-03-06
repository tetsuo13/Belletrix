CREATE TABLE programs (
id              SERIAL,
name            VARCHAR(128) NOT NULL,
abbreviation    VARCHAR(24),

PRIMARY KEY (id),
UNIQUE(name, abbreviation)
);

GRANT ALL PRIVILEGES ON programs TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON programs_id_seq TO neoanime_abroadadvisor;

INSERT INTO programs
(name, abbreviation)
VALUES
('Organization for Tropical Studies', 'OTS'),
('Multidisciplinary International Research Training program', 'MIRT'),
('American Institute for Foreign Study', 'AIFS'),
('New York University', 'NYU'),
('Brethren Colleges Abroad', 'BCA'),
('School for International Training', 'SIT'),
('International Study Program', 'ISP'),
('International Exchange Student Program', 'ISEP'),
('University of Virgin Islands', 'UVI'),
('University of North Carolina at Chapel Hill', 'UNC'),
('Global Learning Semesters', NULL),
('Semester At Sea', NULL),
('Veritas Universidad', NULL),
('UMC Volunteer', NULL),
('Spring Break', NULL),
('GlobaLinks', NULL),
('Scranton Women''s Leadership Cntr.', NULL),
('Bennett Maymester', NULL),
('Global Bus. Leadership Experience', NULL),
('UN Climate Change Conference', NULL),
('New Media', NULL),
('NYU Florence', NULL),
('NYU Ghana', NULL),
('Grahamstown Festival', NULL);


CREATE TABLE program_types (
id          SERIAL,
name        VARCHAR(32) NOT NULL,
short_term  BOOLEAN NOT NULL,

PRIMARY KEY (id),
UNIQUE (name)
);

GRANT ALL PRIVILEGES ON program_types TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON program_types_id_seq TO neoanime_abroadadvisor;

INSERT INTO program_types
(name, short_term)
VALUES
('Maymester', TRUE),
('Spring Break', TRUE),
('Internship', TRUE),
('Research Experience (Short-Term)', TRUE),
('Research Experience (Long-Term)', FALSE),
('Semester', FALSE),
('Summer', TRUE),
('Other', TRUE),
('Academic Year', FALSE);


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

CREATE TABLE regions (
id          SERIAL,
name        VARCHAR(64) NOT NULL,
country_id  INT NOT NULL,

PRIMARY KEY (id),
FOREIGN KEY (country_id) REFERENCES countries (id),
UNIQUE (name, country_id)
);

GRANT ALL PRIVILEGES ON regions TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON regions_id_seq TO neoanime_abroadadvisor;

CREATE TABLE study_abroad (
id                  SERIAL NOT NULL,
student_id          INT NOT NULL,
semester            INT NOT NULL,
year                INT NOT NULL,
start_date          DATE,
end_date            DATE,
credit_bearing      BOOLEAN NOT NULL,
internship          BOOLEAN NOT NULL,
country_id          INT NOT NULL,
region_id           INT,
program_id          INT NOT NULL,

PRIMARY KEY (id),
UNIQUE (student_id, semester, year),
FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (country_id) REFERENCES countries (id),
FOREIGN KEY (region_id) REFERENCES regions (id),
FOREIGN KEY (program_id) REFERENCES programs (id)
);

GRANT ALL PRIVILEGES ON study_abroad TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON study_abroad_id_seq TO neoanime_abroadadvisor;

CREATE TABLE study_abroad_program_types (
study_abroad_id     INT NOT NULL,
program_type_id     INT NOT NULL,

PRIMARY KEY (study_abroad_id, program_type_id),
FOREIGN KEY (program_type_id) REFERENCES program_types (id)
);

GRANT ALL PRIVILEGES ON study_abroad_program_types TO neoanime_abroadadvisor;