-- StudentStudyAbroadWishlistModel.PeriodValue values were each incremented by
-- one. So old value 3 is now 4, old 2 is now 3, and so on. This could've been
-- a simple "SET period = period + 1" but there's special values that can't be
-- incremented -- 99 CatchAllValue, for example.
UPDATE student_study_abroad_wishlist SET period = 4 WHERE period = 3;
UPDATE student_study_abroad_wishlist SET period = 3 WHERE period = 2;
UPDATE student_study_abroad_wishlist SET period = 2 WHERE period = 1;
UPDATE student_study_abroad_wishlist SET period = 1 WHERE period = 0;

-- County can be zero, drop the foreign key constraint.
ALTER TABLE student_study_abroad_wishlist DROP CONSTRAINT student_study_abroad_wishlist_country_id_fkey;
