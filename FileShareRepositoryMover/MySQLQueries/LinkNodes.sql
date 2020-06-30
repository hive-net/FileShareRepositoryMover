DROP TEMPORARY TABLE IF EXISTS linkNewestNodeDate;

CREATE TEMPORARY TABLE linkNewestNodeDate
SELECT l.field_url_value,
		MAX(n.created) newestNode
	FROM field_data_field_url l
    JOIN node n
		ON n.nid = l.entity_id
        AND n.status = '1'
    WHERE l.deleted = '0'
    AND l.bundle = 'pin'
    GROUP BY l.field_url_value;
    
SELECT n.nid,
		l.field_url_value
	FROM linkNewestNodeDate d
    JOIN field_data_field_url l
		ON d.field_url_value = l.field_url_value
        AND l.deleted = '0'
        AND l.bundle = 'pin'
    JOIN node n
		ON n.nid = l.entity_id
        AND n.status = '1'
        AND n.created = d.newestNode
	ORDER BY n.nid;
    
DROP TEMPORARY TABLE IF EXISTS linkNewestNodeDate;