copy(
	SELECT 
		uuid, localization_class_uuid, key,
		application_name,
		class_name,
		inherited_class_name, full_key,
		translations::json -> 'en' as en,
		translations::json -> 'nl' as nl
	FROM mh_localization.translation_keys_extended
	where inherited = false or overwrites = true
) to 'f:/temp/translations.txt' with header csv delimiter  E'\t';