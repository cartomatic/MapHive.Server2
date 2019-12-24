DROP TABLE IF EXISTS mh_localization.translation_keys_temp;
CREATE TABLE mh_localization.translation_keys_temp
(
    uuid uuid NOT NULL,
    localization_class_uuid uuid NOT NULL,
    key text,
	application_name text,
	class_name text,
	inherited_class_name text,
	full_key text,
	en text,
	nl text,
    CONSTRAINT pk_translation_keys_temp PRIMARY KEY (uuid)
);

copy mh_localization.translation_keys_temp
from 'f:/temp/translations.txt' with header csv delimiter E'\t';

update mh_localization.translation_keys orig
set translations = (orig.translations::jsonb || jsonb_build_object('nl',tmp.nl))::text
from mh_localization.translation_keys_temp tmp
where orig.uuid = tmp.uuid;

DROP TABLE IF EXISTS mh_localization.translation_keys_temp;