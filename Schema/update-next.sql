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
