-- Store all date and times in UTC.
SET TIME ZONE 0;

CREATE TABLE countries (
id              SERIAL,
name            VARCHAR(64) NOT NULL,
abbreviation    VARCHAR(3) NOT NULL,

PRIMARY KEY (id),
UNIQUE (name, abbreviation)
);

GRANT ALL PRIVILEGES ON countries TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON countries_id_seq TO neoanime_abroadadvisor;

INSERT INTO countries
(abbreviation, name)
VALUES
('US', 'United States'),
('CA', 'Canada'),
('AF', 'Afghanistan'),
('AL', 'Albania'),
('DZ', 'Algeria'),
('DS', 'American Samoa'),
('AD', 'Andorra'),
('AO', 'Angola'),
('AI', 'Anguilla'),
('AQ', 'Antarctica'),
('AG', 'Antigua and/or Barbuda'),
('AR', 'Argentina'),
('AM', 'Armenia'),
('AW', 'Aruba'),
('AU', 'Australia'),
('AT', 'Austria'),
('AZ', 'Azerbaijan'),
('BS', 'Bahamas'),
('BH', 'Bahrain'),
('BD', 'Bangladesh'),
('BB', 'Barbados'),
('BY', 'Belarus'),
('BE', 'Belgium'),
('BZ', 'Belize'),
('BJ', 'Benin'),
('BM', 'Bermuda'),
('BT', 'Bhutan'),
('BO', 'Bolivia'),
('BA', 'Bosnia and Herzegovina'),
('BW', 'Botswana'),
('BV', 'Bouvet Island'),
('BR', 'Brazil'),
('IO', 'British lndian Ocean Territory'),
('BN', 'Brunei Darussalam'),
('BG', 'Bulgaria'),
('BF', 'Burkina Faso'),
('BI', 'Burundi'),
('KH', 'Cambodia'),
('CM', 'Cameroon'),
('CV', 'Cape Verde'),
('KY', 'Cayman Islands'),
('CF', 'Central African Republic'),
('TD', 'Chad'),
('CL', 'Chile'),
('CN', 'China'),
('CX', 'Christmas Island'),
('CC', 'Cocos (Keeling) Islands'),
('CO', 'Colombia'),
('KM', 'Comoros'),
('CG', 'Congo'),
('CK', 'Cook Islands'),
('CR', 'Costa Rica'),
('HR', 'Croatia (Hrvatska)'),
('CU', 'Cuba'),
('CY', 'Cyprus'),
('CZ', 'Czech Republic'),
('DK', 'Denmark'),
('DJ', 'Djibouti'),
('DM', 'Dominica'),
('DO', 'Dominican Republic'),
('TP', 'East Timor'),
('EC', 'Ecudaor'),
('EG', 'Egypt'),
('SV', 'El Salvador'),
('GQ', 'Equatorial Guinea'),
('ER', 'Eritrea'),
('EE', 'Estonia'),
('ET', 'Ethiopia'),
('FK', 'Falkland Islands (Malvinas)'),
('FO', 'Faroe Islands'),
('FJ', 'Fiji'),
('FI', 'Finland'),
('FR', 'France'),
('FX', 'France, Metropolitan'),
('GF', 'French Guiana'),
('PF', 'French Polynesia'),
('TF', 'French Southern Territories'),
('GA', 'Gabon'),
('GM', 'Gambia'),
('GE', 'Georgia'),
('DE', 'Germany'),
('GH', 'Ghana'),
('GI', 'Gibraltar'),
('GR', 'Greece'),
('GL', 'Greenland'),
('GD', 'Grenada'),
('GP', 'Guadeloupe'),
('GU', 'Guam'),
('GT', 'Guatemala'),
('GN', 'Guinea'),
('GW', 'Guinea-Bissau'),
('GY', 'Guyana'),
('HT', 'Haiti'),
('HM', 'Heard and Mc Donald Islands'),
('HN', 'Honduras'),
('HK', 'Hong Kong'),
('HU', 'Hungary'),
('IS', 'Iceland'),
('IN', 'India'),
('ID', 'Indonesia'),
('IR', 'Iran (Islamic Republic of)'),
('IQ', 'Iraq'),
('IE', 'Ireland'),
('IL', 'Israel'),
('IT', 'Italy'),
('CI', 'Ivory Coast'),
('JM', 'Jamaica'),
('JP', 'Japan'),
('JO', 'Jordan'),
('KZ', 'Kazakhstan'),
('KE', 'Kenya'),
('KI', 'Kiribati'),
('KP', 'Korea, Democratic People''s Republic of'),
('KR', 'Korea, Republic of'),
('KW', 'Kuwait'),
('KG', 'Kyrgyzstan'),
('LA', 'Lao People''s Democratic Republic'),
('LV', 'Latvia'),
('LB', 'Lebanon'),
('LS', 'Lesotho'),
('LR', 'Liberia'),
('LY', 'Libyan Arab Jamahiriya'),
('LI', 'Liechtenstein'),
('LT', 'Lithuania'),
('LU', 'Luxembourg'),
('MO', 'Macau'),
('MK', 'Macedonia'),
('MG', 'Madagascar'),
('MW', 'Malawi'),
('MY', 'Malaysia'),
('MV', 'Maldives'),
('ML', 'Mali'),
('MT', 'Malta'),
('MH', 'Marshall Islands'),
('MQ', 'Martinique'),
('MR', 'Mauritania'),
('MU', 'Mauritius'),
('TY', 'Mayotte'),
('MX', 'Mexico'),
('FM', 'Micronesia, Federated States of'),
('MD', 'Moldova, Republic of'),
('MC', 'Monaco'),
('MN', 'Mongolia'),
('MS', 'Montserrat'),
('MA', 'Morocco'),
('MZ', 'Mozambique'),
('MM', 'Myanmar'),
('NA', 'Namibia'),
('NR', 'Nauru'),
('NP', 'Nepal'),
('NL', 'Netherlands'),
('AN', 'Netherlands Antilles'),
('NC', 'New Caledonia'),
('NZ', 'New Zealand'),
('NI', 'Nicaragua'),
('NE', 'Niger'),
('NG', 'Nigeria'),
('NU', 'Niue'),
('NF', 'Norfork Island'),
('MP', 'Northern Mariana Islands'),
('NO', 'Norway'),
('OM', 'Oman'),
('PK', 'Pakistan'),
('PW', 'Palau'),
('PA', 'Panama'),
('PG', 'Papua New Guinea'),
('PY', 'Paraguay'),
('PE', 'Peru'),
('PH', 'Philippines'),
('PN', 'Pitcairn'),
('PL', 'Poland'),
('PT', 'Portugal'),
('PR', 'Puerto Rico'),
('QA', 'Qatar'),
('RE', 'Reunion'),
('RO', 'Romania'),
('RU', 'Russian Federation'),
('RW', 'Rwanda'),
('KN', 'Saint Kitts and Nevis'),
('LC', 'Saint Lucia'),
('VC', 'Saint Vincent and the Grenadines'),
('WS', 'Samoa'),
('SM', 'San Marino'),
('ST', 'Sao Tome and Principe'),
('SA', 'Saudi Arabia'),
('SN', 'Senegal'),
('SC', 'Seychelles'),
('SL', 'Sierra Leone'),
('SG', 'Singapore'),
('SK', 'Slovakia'),
('SI', 'Slovenia'),
('SB', 'Solomon Islands'),
('SO', 'Somalia'),
('ZA', 'South Africa'),
('GS', 'South Georgia South Sandwich Islands'),
('ES', 'Spain'),
('LK', 'Sri Lanka'),
('SH', 'St. Helena'),
('PM', 'St. Pierre and Miquelon'),
('SD', 'Sudan'),
('SR', 'Suriname'),
('SJ', 'Svalbarn and Jan Mayen Islands'),
('SZ', 'Swaziland'),
('SE', 'Sweden'),
('CH', 'Switzerland'),
('SY', 'Syrian Arab Republic'),
('TW', 'Taiwan'),
('TJ', 'Tajikistan'),
('TZ', 'Tanzania, United Republic of'),
('TH', 'Thailand'),
('TG', 'Togo'),
('TK', 'Tokelau'),
('TO', 'Tonga'),
('TT', 'Trinidad and Tobago'),
('TN', 'Tunisia'),
('TR', 'Turkey'),
('TM', 'Turkmenistan'),
('TC', 'Turks and Caicos Islands'),
('TV', 'Tuvalu'),
('UG', 'Uganda'),
('UA', 'Ukraine'),
('AE', 'United Arab Emirates'),
('GB', 'United Kingdom'),
('UM', 'United States minor outlying islands'),
('UY', 'Uruguay'),
('UZ', 'Uzbekistan'),
('VU', 'Vanuatu'),
('VA', 'Vatican City State'),
('VE', 'Venezuela'),
('VN', 'Vietnam'),
('VG', 'Virigan Islands (British)'),
('VI', 'Virgin Islands (U.S.)'),
('WF', 'Wallis and Futuna Islands'),
('EH', 'Western Sahara'),
('YE', 'Yemen'),
('YU', 'Yugoslavia'),
('ZR', 'Zaire'),
('ZM', 'Zambia'),
('ZW', 'Zimbabwe');

CREATE TABLE dorms (
id          SERIAL,
hall_name   VARCHAR(32),

PRIMARY KEY (id),
UNIQUE (hall_name)
);

INSERT INTO dorms
(hall_name)
VALUES
('Barge'),
('Jones'),
('Pfeiffer'),
('Reynolds'),
('Cone'),
('Honors'),
('Player');

GRANT ALL PRIVILEGES ON dorms TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON dorms_id_seq TO neoanime_abroadadvisor;


CREATE TABLE majors (
id      SERIAL,
name    VARCHAR(128) NOT NULL,

PRIMARY KEY (id),
UNIQUE (name)
);

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
('Women''s Studies');


CREATE TABLE students (
id                      SERIAL,
created                 TIMESTAMP NOT NULL,
initial_meeting         DATE,
first_name              VARCHAR(64) NOT NULL,
middle_name             VARCHAR(64),
last_name               VARCHAR(64) NOT NULL,
living_on_campus        BOOLEAN,
street_address          VARCHAR(128),
street_address2         VARCHAR(128),
city                    VARCHAR(128),
state                   VARCHAR(32),
postal_code             VARCHAR(16),
phone_number            VARCHAR(32),
cell_phone_number       VARCHAR(32),
classification          INT,
student_id              VARCHAR(32),
dob                     DATE,
dorm_id                 INTEGER,
room_number             VARCHAR(8),
campus_po_box           VARCHAR(16),
enrolled_full_time      BOOLEAN,
citizenship             INTEGER,
pell_grant_recipient    BOOLEAN,
passport_holder         BOOLEAN,
gpa                     DECIMAL(3,2),
campus_email            VARCHAR(128),
alternate_email         VARCHAR(128),
major_id                INT,
minor_id                INT,

PRIMARY KEY (id),
FOREIGN KEY (dorm_id) REFERENCES dorms (id),
FOREIGN KEY (citizenship) REFERENCES countries (id),
FOREIGN KEY (major_id) REFERENCES majors (id),
FOREIGN KEY (minor_id) REFERENCES majors (id)
);

GRANT ALL PRIVILEGES ON students TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON students_id_seq TO neoanime_abroadadvisor;


-- http://stackoverflow.com/a/6673029
CREATE TABLE users (
id          SERIAL,
first_name  VARCHAR(64) NOT NULL,
last_name   VARCHAR(64) NOT NULL,
login       VARCHAR(24) NOT NULL,
password    CHAR(256) NOT NULL,
created     TIMESTAMP NOT NULL,
last_login  TIMESTAMP,
email       VARCHAR(128) NOT NULL,
admin       BOOLEAN NOT NULL DEFAULT FALSE,
active      BOOLEAN NOT NULL DEFAULT TRUE,

PRIMARY KEY (id),
UNIQUE (login)
);

GRANT ALL PRIVILEGES ON users TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON users_id_seq TO neoanime_abroadadvisor;

-- Password is the same as the login.
INSERT INTO users
(first_name, last_name, login, password, created, email, admin)
VALUES
('Andrei', 'Nicholson', 'anicholson', '741a86856f1185fb9e29cee6d95e978177147787060e1ec42d0e88b516214ebd', NOW(), 'contact@andreinicholson.com', TRUE);


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

GRANT ALL PRIVILEGES ON student_notes TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON student_notes_id_seq TO neoanime_abroadadvisor;


CREATE TABLE event_log (
id          SERIAL,
date        TIMESTAMP NOT NULL,
modified_by INT NOT NULL,
student_id  INT,
user_id     INT,
type        INT NOT NULL,
action      VARCHAR(512),

PRIMARY KEY (id),
FOREIGN KEY (modified_by) REFERENCES users (id),
FOREIGN KEY (student_id) REFERENCES students (id),
FOREIGN KEY (user_id) REFERENCES users (id)
);

GRANT ALL PRIVILEGES ON event_log TO neoanime_abroadadvisor;
GRANT ALL PRIVILEGES ON event_log_id_seq TO neoanime_abroadadvisor;
