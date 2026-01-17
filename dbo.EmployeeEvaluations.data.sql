SET IDENTITY_INSERT [dbo].[EmployeeEvaluations] ON
INSERT INTO [dbo].[EmployeeEvaluations] ([Id], [EmployeeId], [ManagerId], [Attendance], [Quality], [Productivity], [Teamwork], [Comments], [FinalScore], [CreatedAt], [Period]) VALUES (1, 6, N'c448faf3-53d5-411f-91c6-2a28ef0fbd94', 4, 5, 4, 5, N'Great improvement this quarter.', CAST(4.50 AS Decimal(18, 2)), N'2025-12-08 20:51:38', N'')
INSERT INTO [dbo].[EmployeeEvaluations] ([Id], [EmployeeId], [ManagerId], [Attendance], [Quality], [Productivity], [Teamwork], [Comments], [FinalScore], [CreatedAt], [Period]) VALUES (2, 7, N'c448faf3-53d5-411f-91c6-2a28ef0fbd94', 4, 5, 3, 5, N'Great improvement this quarter.', CAST(4.25 AS Decimal(18, 2)), N'2025-12-08 20:53:30', N'')
INSERT INTO [dbo].[EmployeeEvaluations] ([Id], [EmployeeId], [ManagerId], [Attendance], [Quality], [Productivity], [Teamwork], [Comments], [FinalScore], [CreatedAt], [Period]) VALUES (3, 8, N'c448faf3-53d5-411f-91c6-2a28ef0fbd94', 4, 4, 3, 5, N'Great improvement this quarter.', CAST(4.00 AS Decimal(18, 2)), N'2025-12-08 20:53:50', N'')
INSERT INTO [dbo].[EmployeeEvaluations] ([Id], [EmployeeId], [ManagerId], [Attendance], [Quality], [Productivity], [Teamwork], [Comments], [FinalScore], [CreatedAt], [Period]) VALUES (4, 9, N'c448faf3-53d5-411f-91c6-2a28ef0fbd94', 4, 4, 5, 5, N'Great improvement this quarter.', CAST(4.50 AS Decimal(18, 2)), N'2025-12-08 20:54:09', N'')
INSERT INTO [dbo].[EmployeeEvaluations] ([Id], [EmployeeId], [ManagerId], [Attendance], [Quality], [Productivity], [Teamwork], [Comments], [FinalScore], [CreatedAt], [Period]) VALUES (5, 10, N'c448faf3-53d5-411f-91c6-2a28ef0fbd94', 4, 4, 5, 5, N'Great improvement this quarter.', CAST(4.50 AS Decimal(18, 2)), N'2025-12-08 20:54:22', N'')
SET IDENTITY_INSERT [dbo].[EmployeeEvaluations] OFF
UPDATE EmployeeEvaluations
SET Period = FORMAT(CreatedAt, 'yyyy-MM-dd')

