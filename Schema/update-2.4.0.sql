COMMENT ON TABLE student_promo_log IS 'Students associated with promos';

-- A student may be associated with more than one promo.
ALTER TABLE student_promo_log DROP CONSTRAINT student_promo_log_pkey;
ALTER TABLE student_promo_log ADD PRIMARY KEY (promo_id, student_id);
CREATE INDEX student_promo_log_idx2 ON student_promo_log (student_id);
