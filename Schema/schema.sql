-- Store all date and times in UTC.
SET TIME ZONE 0;

CREATE TABLE countries (
id              SERIAL,
name            VARCHAR(64) NOT NULL,
abbreviation    VARCHAR(3) NOT NULL,
is_region       BOOLEAN NOT NULL,

PRIMARY KEY (id),
UNIQUE (name, abbreviation, is_region)
);

GRANT ALL PRIVILEGES ON countries TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON countries_id_seq TO neoanime_abroadadvisor;

COMMENT ON TABLE countries IS 'Countries in ISO 3166-1 and regions';

INSERT INTO countries
(abbreviation, is_region, name)
VALUES
('US', FALSE, 'United States'),
('CA', FALSE, 'Canada'),
('AF', FALSE, 'Afghanistan'),
('AL', FALSE, 'Albania'),
('DZ', FALSE, 'Algeria'),
('DS', FALSE, 'American Samoa'),
('AD', FALSE, 'Andorra'),
('AO', FALSE, 'Angola'),
('AI', FALSE, 'Anguilla'),
('AQ', FALSE, 'Antarctica'),
('AG', FALSE, 'Antigua and/or Barbuda'),
('AR', FALSE, 'Argentina'),
('AM', FALSE, 'Armenia'),
('AW', FALSE, 'Aruba'),
('AU', FALSE, 'Australia'),
('AT', FALSE, 'Austria'),
('AZ', FALSE, 'Azerbaijan'),
('BS', FALSE, 'Bahamas'),
('BH', FALSE, 'Bahrain'),
('BD', FALSE, 'Bangladesh'),
('BB', FALSE, 'Barbados'),
('BY', FALSE, 'Belarus'),
('BE', FALSE, 'Belgium'),
('BZ', FALSE, 'Belize'),
('BJ', FALSE, 'Benin'),
('BM', FALSE, 'Bermuda'),
('BT', FALSE, 'Bhutan'),
('BO', FALSE, 'Bolivia'),
('BA', FALSE, 'Bosnia and Herzegovina'),
('BW', FALSE, 'Botswana'),
('BV', FALSE, 'Bouvet Island'),
('BR', FALSE, 'Brazil'),
('IO', FALSE, 'British lndian Ocean Territory'),
('BN', FALSE, 'Brunei Darussalam'),
('BG', FALSE, 'Bulgaria'),
('BF', FALSE, 'Burkina Faso'),
('BI', FALSE, 'Burundi'),
('KH', FALSE, 'Cambodia'),
('CM', FALSE, 'Cameroon'),
('CV', FALSE, 'Cape Verde'),
('KY', FALSE, 'Cayman Islands'),
('CF', FALSE, 'Central African Republic'),
('TD', FALSE, 'Chad'),
('CL', FALSE, 'Chile'),
('CN', FALSE, 'China'),
('CX', FALSE, 'Christmas Island'),
('CC', FALSE, 'Cocos (Keeling) Islands'),
('CO', FALSE, 'Colombia'),
('KM', FALSE, 'Comoros'),
('CG', FALSE, 'Congo'),
('CK', FALSE, 'Cook Islands'),
('CR', FALSE, 'Costa Rica'),
('HR', FALSE, 'Croatia (Hrvatska)'),
('CU', FALSE, 'Cuba'),
('CY', FALSE, 'Cyprus'),
('CZ', FALSE, 'Czech Republic'),
('DK', FALSE, 'Denmark'),
('DJ', FALSE, 'Djibouti'),
('DM', FALSE, 'Dominica'),
('DO', FALSE, 'Dominican Republic'),
('TP', FALSE, 'East Timor'),
('EC', FALSE, 'Ecuador'),
('EG', FALSE, 'Egypt'),
('SV', FALSE, 'El Salvador'),
('GQ', FALSE, 'Equatorial Guinea'),
('ER', FALSE, 'Eritrea'),
('EE', FALSE, 'Estonia'),
('ET', FALSE, 'Ethiopia'),
('FK', FALSE, 'Falkland Islands (Malvinas)'),
('FO', FALSE, 'Faroe Islands'),
('FJ', FALSE, 'Fiji'),
('FI', FALSE, 'Finland'),
('FR', FALSE, 'France'),
('FX', FALSE, 'France, Metropolitan'),
('GF', FALSE, 'French Guiana'),
('PF', FALSE, 'French Polynesia'),
('TF', FALSE, 'French Southern Territories'),
('GA', FALSE, 'Gabon'),
('GM', FALSE, 'Gambia'),
('GE', FALSE, 'Georgia'),
('DE', FALSE, 'Germany'),
('GH', FALSE, 'Ghana'),
('GI', FALSE, 'Gibraltar'),
('GR', FALSE, 'Greece'),
('GL', FALSE, 'Greenland'),
('GD', FALSE, 'Grenada'),
('GP', FALSE, 'Guadeloupe'),
('GU', FALSE, 'Guam'),
('GT', FALSE, 'Guatemala'),
('GN', FALSE, 'Guinea'),
('GW', FALSE, 'Guinea-Bissau'),
('GY', FALSE, 'Guyana'),
('HT', FALSE, 'Haiti'),
('HM', FALSE, 'Heard and Mc Donald Islands'),
('HN', FALSE, 'Honduras'),
('HK', FALSE, 'Hong Kong'),
('HU', FALSE, 'Hungary'),
('IS', FALSE, 'Iceland'),
('IN', FALSE, 'India'),
('ID', FALSE, 'Indonesia'),
('IR', FALSE, 'Iran'),
('IQ', FALSE, 'Iraq'),
('IE', FALSE, 'Ireland'),
('IL', FALSE, 'Israel'),
('IT', FALSE, 'Italy'),
('CI', FALSE, 'Ivory Coast'),
('JM', FALSE, 'Jamaica'),
('JP', FALSE, 'Japan'),
('JO', FALSE, 'Jordan'),
('KZ', FALSE, 'Kazakhstan'),
('KE', FALSE, 'Kenya'),
('KI', FALSE, 'Kiribati'),
('KP', FALSE, 'North Korea'),
('KR', FALSE, 'South Korea'),
('KW', FALSE, 'Kuwait'),
('KG', FALSE, 'Kyrgyzstan'),
('LA', FALSE, 'Laos'),
('LV', FALSE, 'Latvia'),
('LB', FALSE, 'Lebanon'),
('LS', FALSE, 'Lesotho'),
('LR', FALSE, 'Liberia'),
('LY', FALSE, 'Libya'),
('LI', FALSE, 'Liechtenstein'),
('LT', FALSE, 'Lithuania'),
('LU', FALSE, 'Luxembourg'),
('MO', FALSE, 'Macau'),
('MK', FALSE, 'Macedonia'),
('MG', FALSE, 'Madagascar'),
('MW', FALSE, 'Malawi'),
('MY', FALSE, 'Malaysia'),
('MV', FALSE, 'Maldives'),
('ML', FALSE, 'Mali'),
('MT', FALSE, 'Malta'),
('MH', FALSE, 'Marshall Islands'),
('MQ', FALSE, 'Martinique'),
('MR', FALSE, 'Mauritania'),
('MU', FALSE, 'Mauritius'),
('TY', FALSE, 'Mayotte'),
('MX', FALSE, 'Mexico'),
('FM', FALSE, 'Micronesia'),
('MD', FALSE, 'Moldova'),
('MC', FALSE, 'Monaco'),
('MN', FALSE, 'Mongolia'),
('MS', FALSE, 'Montserrat'),
('MA', FALSE, 'Morocco'),
('MZ', FALSE, 'Mozambique'),
('MM', FALSE, 'Myanmar'),
('NA', FALSE, 'Namibia'),
('NR', FALSE, 'Nauru'),
('NP', FALSE, 'Nepal'),
('NL', FALSE, 'Netherlands'),
('AN', FALSE, 'Netherlands Antilles'),
('NC', FALSE, 'New Caledonia'),
('NZ', FALSE, 'New Zealand'),
('NI', FALSE, 'Nicaragua'),
('NE', FALSE, 'Niger'),
('NG', FALSE, 'Nigeria'),
('NU', FALSE, 'Niue'),
('NF', FALSE, 'Norfolk Island'),
('MP', FALSE, 'Northern Mariana Islands'),
('NO', FALSE, 'Norway'),
('OM', FALSE, 'Oman'),
('PK', FALSE, 'Pakistan'),
('PW', FALSE, 'Palau'),
('PA', FALSE, 'Panama'),
('PG', FALSE, 'Papua New Guinea'),
('PY', FALSE, 'Paraguay'),
('PE', FALSE, 'Peru'),
('PH', FALSE, 'Philippines'),
('PN', FALSE, 'Pitcairn'),
('PL', FALSE, 'Poland'),
('PT', FALSE, 'Portugal'),
('PR', FALSE, 'Puerto Rico'),
('QA', FALSE, 'Qatar'),
('RE', FALSE, 'Reunion'),
('RO', FALSE, 'Romania'),
('RU', FALSE, 'Russian Federation'),
('RW', FALSE, 'Rwanda'),
('KN', FALSE, 'Saint Kitts and Nevis'),
('LC', FALSE, 'Saint Lucia'),
('VC', FALSE, 'Saint Vincent and the Grenadines'),
('WS', FALSE, 'Samoa'),
('SM', FALSE, 'San Marino'),
('ST', FALSE, 'Sao Tome and Principe'),
('SA', FALSE, 'Saudi Arabia'),
('SN', FALSE, 'Senegal'),
('SC', FALSE, 'Seychelles'),
('SL', FALSE, 'Sierra Leone'),
('SG', FALSE, 'Singapore'),
('SK', FALSE, 'Slovakia'),
('SI', FALSE, 'Slovenia'),
('SB', FALSE, 'Solomon Islands'),
('SO', FALSE, 'Somalia'),
('ZA', FALSE, 'South Africa'),
('GS', FALSE, 'South Georgia South Sandwich Islands'),
('ES', FALSE, 'Spain'),
('LK', FALSE, 'Sri Lanka'),
('SH', FALSE, 'Saint Helena'),
('PM', FALSE, 'Saint Pierre and Miquelon'),
('SD', FALSE, 'Sudan'),
('SR', FALSE, 'Suriname'),
('SJ', FALSE, 'Svalbard and Jan Mayen Islands'),
('SZ', FALSE, 'Swaziland'),
('SE', FALSE, 'Sweden'),
('CH', FALSE, 'Switzerland'),
('SY', FALSE, 'Syria'),
('TW', FALSE, 'Taiwan'),
('TJ', FALSE, 'Tajikistan'),
('TZ', FALSE, 'Tanzania'),
('TH', FALSE, 'Thailand'),
('TG', FALSE, 'Togo'),
('TK', FALSE, 'Tokelau'),
('TO', FALSE, 'Tonga'),
('TT', FALSE, 'Trinidad and Tobago'),
('TN', FALSE, 'Tunisia'),
('TR', FALSE, 'Turkey'),
('TM', FALSE, 'Turkmenistan'),
('TC', FALSE, 'Turks and Caicos Islands'),
('TV', FALSE, 'Tuvalu'),
('UG', FALSE, 'Uganda'),
('UA', FALSE, 'Ukraine'),
('AE', FALSE, 'United Arab Emirates'),
('GB', FALSE, 'United Kingdom'),
('UM', FALSE, 'United States minor outlying islands'),
('UY', FALSE, 'Uruguay'),
('UZ', FALSE, 'Uzbekistan'),
('VU', FALSE, 'Vanuatu'),
('VA', FALSE, 'Vatican City'),
('VE', FALSE, 'Venezuela'),
('VN', FALSE, 'Vietnam'),
('VG', FALSE, 'Virgin Islands (British)'),
('VI', FALSE, 'Virgin Islands (U.S.)'),
('WF', FALSE, 'Wallis and Futuna Islands'),
('EH', FALSE, 'Western Sahara'),
('YE', FALSE, 'Yemen'),
('YU', FALSE, 'Yugoslavia'),
('ZR', FALSE, 'Zaire'),
('ZM', FALSE, 'Zambia'),
('ZW', FALSE, 'Zimbabwe'),
('', FALSE, 'Anywhere'),
('EU', TRUE, 'Europe'),
('AS', TRUE, 'Asia'),
('AF', TRUE, 'Africa'),
('LA', TRUE, 'Latin America'),
('ES', TRUE, 'English-speaking');


CREATE TABLE majors (
id      SERIAL,
name    VARCHAR(128) NOT NULL,

PRIMARY KEY (id),
UNIQUE (name)
);

COMMENT ON TABLE majors IS 'Available majors';

GRANT ALL PRIVILEGES ON majors TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON majors_id_seq TO neoanime_abroadadvisor;

INSERT INTO majors
(name)
VALUES
('Arts Management'),
('Biology'),
('Business'),
('Business Administration'),
('Business Management'),
('Chemistry'),
('Computer Science'),
('Early Childhood'),
('Education'),
('English'),
('Interdisciplinary Studies'),
('International Business'),
('Journalism and Media Studies'),
('Political Science'),
('Psychology'),
('Social Work'),
('Special Education'),
('Theater'),
('Women''s Studies'),
('Undecided & Entrepreneurial Studies');


CREATE TABLE minors (
id      SERIAL,
name    VARCHAR(128) NOT NULL,

PRIMARY KEY (id),
UNIQUE (name)
);

COMMENT ON TABLE minors IS 'Available minors';

GRANT ALL PRIVILEGES ON minors TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON minors_id_seq TO neoanime_abroadadvisor;

INSERT INTO minors
(name)
VALUES
('Arts Management'),
('Biology'),
('Business'),
('Business Administration'),
('Business Management'),
('Chemistry'),
('Computer Science'),
('Early Childhood'),
('Education'),
('English'),
('Interdisciplinary Studies'),
('International Business'),
('Journalism and Media Studies'),
('Political Science'),
('Psychology'),
('Social Work'),
('Special Education'),
('Theater'),
('Women''s Studies'),
('Undecided & Entrepreneurial Studies'),
('Entrepreneurial Studies'),
('Global Studies'),
('Spanish');


CREATE TABLE languages (
id          SERIAL,
name        VARCHAR(32) NOT NULL,

PRIMARY KEY (id),
UNIQUE (name)
);

GRANT ALL PRIVILEGES ON languages TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON languages_id_seq TO neoanime_abroadadvisor;

INSERT INTO languages
(name)
VALUES
('Mandarin'),
('Spanish'),
('English'),
('Hindi'),
('Arabic'),
('Portuguese'),
('Bengali'),
('Russian'),
('Japanese'),
('Punjabi'),
('German'),
('Javanese'),
('Wu'),
('Malay/Indonesian'),
('Telugu'),
('Vietnamese'),
('Korean'),
('French'),
('Marathi'),
('Tamil'),
('Urdu'),
('Persian'),
('Turkish'),
('Italian'),
('Cantonese'),
('Hausa'),
('Kiswahili');


CREATE TABLE students (
id                      SERIAL,
created                 TIMESTAMP NOT NULL,
initial_meeting         DATE,
first_name              VARCHAR(64) NOT NULL,
middle_name             VARCHAR(64),
last_name               VARCHAR(64) NOT NULL,
street_address          VARCHAR(128),
street_address2         VARCHAR(128),
city                    VARCHAR(128),
state                   VARCHAR(32),
postal_code             VARCHAR(16),
classification          INT,
student_id              VARCHAR(32),
phone_number            VARCHAR(32),
living_on_campus        BOOLEAN,
enrolled_full_time      BOOLEAN,
citizenship             INTEGER,
pell_grant_recipient    BOOLEAN,
passport_holder         BOOLEAN,
phi_beta_delta_member   BOOLEAN,
gpa                     DECIMAL(3,2),
campus_email            VARCHAR(128),
alternate_email         VARCHAR(128),
entering_year           INT,
graduating_year         INT,
dob                     DATE,

PRIMARY KEY (id),
FOREIGN KEY (citizenship) REFERENCES countries (id)
);

COMMENT ON TABLE students IS 'Student master';
COMMENT ON COLUMN students.graduating_year IS 'Year that student will be or has already graduated';

GRANT ALL PRIVILEGES ON students TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON students_id_seq TO neoanime_abroadadvisor;


CREATE TABLE student_fluent_languages (
student_id      INT NOT NULL,
language_id     INT NOT NULL,

PRIMARY KEY (student_id, language_id)
);

COMMENT ON TABLE student_fluent_languages IS 'Languages that students declare fluency';

GRANT ALL PRIVILEGES ON student_fluent_languages TO neoanime_abroadadvisor;


CREATE TABLE student_desired_languages (
student_id      INT NOT NULL,
language_id     INT NOT NULL,

PRIMARY KEY (student_id, language_id)
);

COMMENT ON TABLE student_desired_languages IS 'Languages that students would like to study abroad';

GRANT ALL PRIVILEGES ON student_desired_languages TO neoanime_abroadadvisor;


CREATE TABLE student_studied_languages (
student_id      INT NOT NULL,
language_id     INT NOT NULL,

PRIMARY KEY (student_id, language_id)
);

COMMENT ON TABLE student_studied_languages IS 'Languages that students have studied or are studying';

GRANT ALL ON TABLE student_studied_languages TO neoanime_abroadadvisor;


CREATE TABLE matriculation (
student_id  INT NOT NULL,
major_id    INT NOT NULL,
is_major    BOOLEAN NOT NULL,

PRIMARY KEY (student_id, major_id, is_major)
);

COMMENT ON TABLE matriculation IS 'Student major/minor cross reference';

GRANT ALL PRIVILEGES ON matriculation TO neoanime_abroadadvisor;


CREATE TABLE student_study_abroad_wishlist (
student_id  INT NOT NULL,
country_id  INT,
year        INT,
period      INT,

FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (country_id) REFERENCES countries (id)
);

COMMENT ON TABLE student_study_abroad_wishlist IS 'Location and semesters that students wish to study abroad to';
COMMENT ON COLUMN student_study_abroad_wishlist.student_id IS 'Student ID';
COMMENT ON COLUMN student_study_abroad_wishlist.country_id IS 'Country ID';
COMMENT ON COLUMN student_study_abroad_wishlist.year IS 'Desired four digit year';
COMMENT ON COLUMN student_study_abroad_wishlist.period IS 'One of StudentStudyAbroadWishlistModel.PeriodValue';

CREATE INDEX student_study_abroad_wishlist_idx1 ON student_study_abroad_wishlist (student_id);

GRANT ALL PRIVILEGES ON student_study_abroad_wishlist TO neoanime_abroadadvisor;


CREATE TABLE users (
id                      SERIAL,
first_name              VARCHAR(64) NOT NULL,
last_name               VARCHAR(64) NOT NULL,
login                   VARCHAR(24) NOT NULL,
password_iterations     INTEGER NOT NULL,
password_salt           CHAR(32) NOT NULL,
password_hash           CHAR(32) NOT NULL,
created                 TIMESTAMP NOT NULL,
last_login              TIMESTAMP,
email                   VARCHAR(128) NOT NULL,
admin                   BOOLEAN NOT NULL DEFAULT FALSE,
active                  BOOLEAN NOT NULL DEFAULT TRUE,

PRIMARY KEY (id),
UNIQUE (login)
);

COMMENT ON TABLE users IS 'Logins and information about users of the application';
COMMENT ON COLUMN users.first_name IS 'Given name';
COMMENT ON COLUMN users.last_name IS 'Family name';
COMMENT ON COLUMN users.login IS 'Username used to log into the application';
COMMENT ON COLUMN users.password_hash IS 'Hash of password';
COMMENT ON COLUMN users.created IS 'Date that the user profile was created';
COMMENT ON COLUMN users.last_login IS 'Date that the user last logged in to the application';
COMMENT ON COLUMN users.email IS 'Email address for user';
COMMENT ON COLUMN users.admin IS 'Administrator access?';
COMMENT ON COLUMN users.active IS 'Active status dictates whether the user can even log in';

GRANT ALL PRIVILEGES ON users TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON users_id_seq TO neoanime_abroadadvisor;

-- Password is the same as the login.
INSERT INTO users
(first_name, last_name, login, password_iterations, password_salt, password_hash, created, email, admin)
VALUES
('Andrei', 'Nicholson', 'anicholson', 1000, 'og85e2R6TvXl+8SuOqv3EWTc7eWX3aje', 'RzfdfsXpQQTg1E+n3wnLMEZbjJoEkqEf', NOW(), 'contact@andreinicholson.com', TRUE);


CREATE TABLE user_promo (
id              SERIAL,
description     VARCHAR(256) NOT NULL,
created_by      INT NOT NULL,
created         TIMESTAMP NOT NULL,
code            VARCHAR(32) NOT NULL,
active          BOOLEAN NOT NULL DEFAULT TRUE,

PRIMARY KEY (id),
UNIQUE (code),
FOREIGN KEY (created_by) REFERENCES users (id)
);

COMMENT ON TABLE user_promo IS 'Promotional users attached to student entries';
COMMENT ON COLUMN user_promo.description IS 'Helper description';
COMMENT ON COLUMN user_promo.created_by IS 'User that created the promo';
COMMENT ON COLUMN user_promo.created IS 'Date and time that the promo was created';
COMMENT ON COLUMN user_promo.code IS 'Unique promo code used to log enter the public portion of the site for the student entry forms';
COMMENT ON COLUMN user_promo.active IS 'Whether the promo can still be used';

GRANT ALL PRIVILEGES ON user_promo TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON user_promo_id_seq TO neoanime_abroadadvisor;


CREATE TABLE student_notes (
id          SERIAL,
student_id  INT NOT NULL,
created_by  INT NOT NULL,
entry_date  TIMESTAMP NOT NULL,
note        TEXT NOT NULL,

PRIMARY KEY (id),
FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (created_by) REFERENCES users (id)
);

COMMENT ON TABLE student_notes IS 'Arbitrary notes attached to students';
COMMENT ON COLUMN student_notes.student_id IS 'Student ID for note';
COMMENT ON COLUMN student_notes.created_by IS 'User ID who created the note';
COMMENT ON COLUMN student_notes.entry_date IS 'Date that the note was created';

GRANT ALL PRIVILEGES ON student_notes TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON student_notes_id_seq TO neoanime_abroadadvisor;


CREATE TABLE event_log (
id          SERIAL,
date        TIMESTAMP NOT NULL,
modified_by INT,
student_id  INT,
user_id     INT,
type        INT NOT NULL,
action      VARCHAR(512),

PRIMARY KEY (id),
FOREIGN KEY (modified_by) REFERENCES users (id),
FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (user_id) REFERENCES users (id)
);

COMMENT ON TABLE event_log IS 'Application event logging table';
COMMENT ON COLUMN event_log.modified_by IS 'User ID who initiated the event. May be NULL to indicate promos or system events.';
COMMENT ON COLUMN event_log.student_id IS 'Student ID that was modified';
COMMENT ON COLUMN event_log.user_id IS 'User ID that was modified';

GRANT ALL PRIVILEGES ON event_log TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON event_log_id_seq TO neoanime_abroadadvisor;


CREATE TABLE student_promo_log (
promo_id    INT NOT NULL,
student_id  INT NOT NULL,

PRIMARY KEY (promo_id, student_id),
FOREIGN KEY (promo_id) REFERENCES user_promo (id),
FOREIGN KEY (student_id) REFERENCES students (id)
);

COMMENT ON TABLE student_promo_log IS 'Students created through ptomos';

GRANT ALL PRIVILEGES ON student_promo_log TO neoanime_abroadadvisor;


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
('Grahamstown Festival', NULL),
('Council on International Educational Exchange', 'CIEE'),
('CISabroad', 'CIS'),
('Mid-Atlantic Consortium-Center for Academic Excellence', 'MAC-CAE'),
('NYU London', NULL),
('Institute for Future Global Leaders UVI', NULL),
('Tec de Monterrey', NULL),
('Global Semesters', NULL),
('Global Linkages', NULL);


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
city                VARCHAR(64),
program_id          INT NOT NULL,

PRIMARY KEY (id),
UNIQUE (student_id, semester, year, country_id),
FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (country_id) REFERENCES countries (id),
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
