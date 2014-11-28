-- A student may only be associated with one promo at most.
ALTER TABLE student_promo_log DROP CONSTRAINT student_promo_log_pkey;
ALTER TABLE student_promo_log ADD PRIMARY KEY (student_id);

CREATE INDEX student_promo_log_idx1 ON student_promo_log (promo_id);

ALTER TABLE student_promo_log ADD COLUMN created TIMESTAMP;
ALTER TABLE student_promo_log ALTER COLUMN created SET NOT NULL;
