USE [DB_51435_menudart]
GO

/****** Object:  Table [dbo].[Races]    Script Date: 05/24/2013 20:31:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Races_raceprojector](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[County] [nvarchar](max) NULL,
	[Year] [int] NOT NULL,
	[Total] [int] NOT NULL,
	[White] [int] NOT NULL,
	[Hispanic] [int] NOT NULL,
	[Asian] [int] NOT NULL,
	[Pacific] [int] NOT NULL,
	[Black] [int] NOT NULL,
	[Indian] [int] NOT NULL,
	[Multirace] [int] NOT NULL,
	[WhitePer] [real] NOT NULL,
	[HispanicPer] [real] NOT NULL,
	[AsianPer] [real] NOT NULL,
	[PacificPer] [real] NOT NULL,
	[BlackPer] [real] NOT NULL,
	[IndianPer] [real] NOT NULL,
	[MultiracePer] [real] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO