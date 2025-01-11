SELECT		Date
			, Duration
			, DurationSeconds
			, Note
INTO		#ReqEntry
FROM		CalendarEntry
INNER JOIN	CalendarEntryType
	ON		CalendarEntryType.Id = CalendarEntry.CalendarEntryTypeId
	AND		CalendarEntryType.Id = 1 -- WorkTime
ORDER BY	Date
			, CalendarEntryType.Id

SELECT		Date
			, Duration
			, DurationSeconds
			, Name
			, Note
INTO		#WorkEntry
FROM		CalendarEntry
INNER JOIN	CalendarEntryType
	ON		CalendarEntryType.Id = CalendarEntry.CalendarEntryTypeId
	AND		CalendarEntryType.Id IN (2, 3, 6) -- Office, Mobile, TimeCompensation
ORDER BY	Date
			, CalendarEntryType.Id

SELECT		Date
			, Duration
			, DurationSeconds
			, Name
			, Note
INTO		#LeavEntry
FROM		CalendarEntry
INNER JOIN	CalendarEntryType
	ON		CalendarEntryType.Id = CalendarEntry.CalendarEntryTypeId
	AND		CalendarEntryType.Id IN (4, 5, 7) -- Holiday, Vacation, Ill
ORDER BY	Date
			, CalendarEntryType.Id

--INSERT [WorkTime] ([WorkTimeTypeId], [Date], [ExpectedSeconds], [ActualSeconds], [Note])
SELECT		CASE #WorkEntry.Name WHEN 'Office' THEN 1 WHEN 'Mobile' THEN 2 WHEN 'TimeCompensation' THEN (CASE LEFT(#ReqEntry.Note, CHARINDEX(':', #ReqEntry.Note) - 1) WHEN 'Büro' THEN 1 WHEN 'Mobil' THEN 2 END) END
			, #WorkEntry.Date
			, ISNULL(#ReqEntry.DurationSeconds, 0)
			, CASE #WorkEntry.Name WHEN 'TimeCompensation' THEN 0 ELSE #WorkEntry.DurationSeconds END
			, CASE #WorkEntry.Name WHEN 'TimeCompensation' THEN 'Zeitausgleich' ELSE #WorkEntry.Note END
FROM		#WorkEntry
LEFT JOIN	#ReqEntry
	ON		#ReqEntry.Date = #WorkEntry.Date
ORDER BY	#WorkEntry.Date

--INSERT [Absence] ([AbsenceTypeId], [Date], [DurationSeconds], [Note])
SELECT		CASE Name WHEN 'Holiday' THEN 1 WHEN 'Vacation' THEN 2 WHEN 'Ill' THEN 3 END
			, Date
			, DurationSeconds
			, Note
FROM		#LeavEntry
ORDER BY	Date

DROP TABLE #LeavEntry, #ReqEntry, #WorkEntry
