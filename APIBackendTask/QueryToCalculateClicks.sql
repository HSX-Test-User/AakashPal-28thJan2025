-- we have 2 tables; Zone and ClickZone

SELECT z.Name AS ZoneName, COUNT(c.Id) as ClickCount
FROM Zone z
LEFT JOIN ClickZone c ON c.X BETWEEN z.X1 AND z.X2 AND c.Y BETWEEN z.Y1 AND z.Y2
GROUP BY z.Id