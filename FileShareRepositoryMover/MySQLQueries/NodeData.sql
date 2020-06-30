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
    
DROP TEMPORARY TABLE IF EXISTS nodeFileCounts;
CREATE TEMPORARY TABLE nodeFileCounts
SELECT n.nid,
		COUNT(*) Files
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
	LEFT JOIN field_data_field_image i
		ON i.field_image_fid = m.fid
	WHERE i.field_image_fid IS NULL
    GROUP BY n.nid;
        
DROP TEMPORARY TABLE IF EXISTS fileNewestNodeDate;
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

DROP TEMPORARY TABLE IF EXISTS nodeLinkCounts;
CREATE TEMPORARY TABLE nodeLinkCounts
SELECT n.nid,
		COUNT(*) Links
	FROM linkNewestNodeDate d
    JOIN field_data_field_url l
		ON d.field_url_value = l.field_url_value
        AND l.deleted = '0'
        AND l.bundle = 'pin'
    JOIN node n
		ON n.nid = l.entity_id
        AND n.status = '1'
        AND n.created = d.newestNode;	
    
DROP TEMPORARY TABLE IF EXISTS linkNewestNodeDate;

DROP TEMPORARY TABLE IF EXISTS nodeCounts;
CREATE TEMPORARY TABLE nodeCounts
SELECT n.nid,
		(IFNULL(l.Links, 0) + IFNULL(f.Files, 0)) AS ContentCount
	FROM node n
    LEFT JOIN nodeLinkCounts l
		ON l.nid = n.nid
	LEFT JOIN nodeFileCounts f
		ON f.nid = n.nid
	WHERE (IFNULL(l.Links, 0) + IFNULL(f.Files, 0)) > 0;
    
DROP TEMPORARY TABLE IF EXISTS nodeLinkCounts;
DROP TEMPORARY TABLE IF EXISTS nodeFileCounts;

DROP TEMPORARY TABLE IF EXISTS nodeCategory;
CREATE TEMPORARY TABLE nodeCategory
SELECT n.nid,
		d.tid
	FROM nodeCounts n
	JOIN taxonomy_index i
		ON i.nid = n.nid
    JOIN taxonomy_term_data d
		ON d.tid = i.tid
	JOIN taxonomy_vocabulary v
		ON v.vid = d.vid
	WHERE v.vid = 30
    ORDER BY n.nid;

SELECT n.nid,
		IFNULL(c.tid, 0) AS tid,
        n.ContentCount,
        o.title
	FROM nodeCounts n
    JOIN node o
		ON o.nid = n.nid
    LEFT JOIN nodeCategory c
		ON c.nid = n.nid;

DROP TEMPORARY TABLE IF EXISTS nodeCategory;
DROP TEMPORARY TABLE IF EXISTS nodeCounts;