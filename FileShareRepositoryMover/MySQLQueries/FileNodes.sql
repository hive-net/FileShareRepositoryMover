DROP TEMPORARY TABLE IF EXISTS fileNewestNodeDate;
CREATE TEMPORARY TABLE fileNewestNodeDate
SELECT m.fid,
        MAX(n.created) newestNode
	FROM file_usage u
    JOIN node n
		ON n.type = 'pin'
		AND n.nid = u.id
        AND n.status = '1'
	JOIN file_managed m
		ON m.fid = u.fid
        AND m.status = '1'
	WHERE u.type = 'node'
    GROUP BY m.fid;

SELECT m.fid,
		n.nid,
		n.vid,
        m.filename,
        m.uri,
        m.filemime,
        n.title
	FROM fileNewestNodeDate nf
    JOIN file_managed m
		ON m.fid = nf.fid
        AND m.status = '1'
	JOIN file_usage u
		ON u.fid = m.fid
        AND u.type = 'node'
	JOIN node n
		ON n.nid = u.id
        AND n.type = 'pin'
        AND n.status = '1'
        AND n.created = nf.newestNode
        AND from_unixtime(n.created) >= '2018-01-01'
	LEFT JOIN field_data_field_image i
		ON i.field_image_fid = m.fid
	WHERE i.field_image_fid IS NULL;
        
DROP TEMPORARY TABLE IF EXISTS fileNewestNodeDate;