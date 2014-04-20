ALTER TABLE students DROP COLUMN street_address;
ALTER TABLE students DROP COLUMN street_address2;
ALTER TABLE students DROP COLUMN city;
ALTER TABLE students DROP COLUMN state;
ALTER TABLE students DROP COLUMN postal_code;
ALTER TABLE students DROP COLUMN cell_phone_number;
ALTER TABLE students DROP COLUMN entering_year;
ALTER TABLE students DROP COLUMN classification;
ALTER TABLE students DROP COLUMN phi_beta_delta_member;


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


CREATE TABLE student_promo_log (
promo_id    INT NOT NULL,
student_id  INT NOT NULL,

PRIMARY KEY (promo_id, student_id),
FOREIGN KEY (promo_id) REFERENCES user_promo (id),
FOREIGN KEY (student_id) REFERENCES students (id)
);

COMMENT ON TABLE student_promo_log IS 'Students created through ptomos';

GRANT ALL PRIVILEGES ON student_promo_log TO neoanime_abroadadvisor;


ALTER TABLE student_study_abroad_wishlist DROP CONSTRAINT student_study_abroad_wishlist_pkey;
ALTER TABLE student_study_abroad_wishlist ALTER COLUMN country_id DROP NOT NULL;
ALTER TABLE student_study_abroad_wishlist ALTER COLUMN year DROP NOT NULL;
ALTER TABLE student_study_abroad_wishlist ALTER COLUMN period DROP NOT NULL;
CREATE INDEX student_study_abroad_wishlist_idx1 ON student_study_abroad_wishlist (student_id);
