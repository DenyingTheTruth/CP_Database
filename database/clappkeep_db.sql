USE [master]
GO
/****** Object:  Database [forest-report]    Script Date: 31.05.2021 20:13:44 ******/
CREATE DATABASE [forest-report]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'forest-report', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\forest-report.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'forest-report_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\forest-report_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [forest-report] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [forest-report].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [forest-report] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [forest-report] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [forest-report] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [forest-report] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [forest-report] SET ARITHABORT OFF 
GO
ALTER DATABASE [forest-report] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [forest-report] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [forest-report] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [forest-report] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [forest-report] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [forest-report] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [forest-report] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [forest-report] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [forest-report] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [forest-report] SET  DISABLE_BROKER 
GO
ALTER DATABASE [forest-report] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [forest-report] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [forest-report] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [forest-report] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [forest-report] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [forest-report] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [forest-report] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [forest-report] SET RECOVERY FULL 
GO
ALTER DATABASE [forest-report] SET  MULTI_USER 
GO
ALTER DATABASE [forest-report] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [forest-report] SET DB_CHAINING OFF 
GO
ALTER DATABASE [forest-report] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [forest-report] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [forest-report] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [forest-report] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'forest-report', N'ON'
GO
ALTER DATABASE [forest-report] SET QUERY_STORE = OFF
GO
USE [forest-report]
GO
/****** Object:  UserDefinedFunction [dbo].[Transalater]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[Transalater](@string nvarchar(500))
RETURNS nvarchar(500) 
AS
begin
set @string = replace (@string, N'ый', N'y')
set @string = replace (@string, N'ЫЙ', N'Y')
set @string = replace (@string, N'а', N'a')
set @string = replace (@string, N'б', N'b')
set @string = replace (@string, N'в', N'v')
set @string = replace (@string, N'г', N'g')
set @string = replace (@string, N'д', N'd')
set @string = replace (@string, N'е', N'e')
set @string = replace (@string, N'ё', N'yo')
set @string = replace (@string, N'ж', N'zh')
set @string = replace (@string, N'з', N'z')
set @string = replace (@string, N'и', N'i')
set @string = replace (@string, N'й', N'y')
set @string = replace (@string, N'к', N'k')
set @string = replace (@string, N'л', N'l')
set @string = replace (@string, N'м', N'm')
set @string = replace (@string, N'н', N'n')
set @string = replace (@string, N'о', N'o')
set @string = replace (@string, N'п', N'p')
set @string = replace (@string, N'р', N'r')
set @string = replace (@string, N'с', N's')
set @string = replace (@string, N'т', N't')
set @string = replace (@string, N'у', N'u')
set @string = replace (@string, N'ф', N'f')
set @string = replace (@string, N'х', N'kh')
set @string = replace (@string, N'ц', N'c')
set @string = replace (@string, N'ч', N'ch')
set @string = replace (@string, N'ш', N'sh')
set @string = replace (@string, N'щ', N'shch')
set @string = replace (@string, N'ъ', N' ')
set @string = replace (@string, N'ы', N'y')
set @string = replace (@string, N'ь', N'')
set @string = replace (@string, N'э', N'e')
set @string = replace (@string, N'ю', N'yu')
set @string = replace (@string, N'я', N'ya')
set @string = replace (@string, N'А', N'A')
set @string = replace (@string, N'Б', N'B')
set @string = replace (@string, N'В', N'V')
set @string = replace (@string, N'Г', N'G')
set @string = replace (@string, N'Д', N'D')
set @string = replace (@string, N'Е', N'E')
set @string = replace (@string, N'Ё', N'YO')
set @string = replace (@string, N'Ж', N'ZH')
set @string = replace (@string, N'З', N'Z')
set @string = replace (@string, N'И', N'I')
set @string = replace (@string, N'Й', N'Y')
set @string = replace (@string, N'К', N'K')
set @string = replace (@string, N'Л', N'L')
set @string = replace (@string, N'М', N'M')
set @string = replace (@string, N'Н', N'N')
set @string = replace (@string, N'О', N'O')
set @string = replace (@string, N'П', N'P')
set @string = replace (@string, N'Р', N'R')
set @string = replace (@string, N'С', N'S')
set @string = replace (@string, N'Т', N'T')
set @string = replace (@string, N'У', N'U')
set @string = replace (@string, N'Ф', N'F')
set @string = replace (@string, N'Х', N'KH')
set @string = replace (@string, N'Ц', N'C')
set @string = replace (@string, N'Ч', N'CH')
set @string = replace (@string, N'Ш', N'SH')
set @string = replace (@string, N'Щ', N'SHCH')
set @string = replace (@string, N'Ъ', N'')
set @string = replace (@string, N'Ы', N'Y')
set @string = replace (@string, N'Ь', N'')
set @string = replace (@string, N'Э', N'E')
set @string = replace (@string, N'Ю', N'YU')
set @string = replace (@string, N'Я', N'YA')
return @String
end
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[OrganizationId] [nvarchar](450) NULL,
	[PasswordEncrypt] [nvarchar](max) NULL,
	[Fio] [nvarchar](max) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataHistory]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataHistory](
	[Id] [nvarchar](450) NOT NULL,
	[OldData] [nvarchar](4000) NULL,
	[NewData] [nvarchar](4000) NULL,
	[TableName] [nvarchar](4000) NULL,
	[HistoryDate] [datetime] NULL,
 CONSTRAINT [PK_DataHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Files]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[Id] [nvarchar](450) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[Value] [varbinary](max) NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogReports]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogReports](
	[Id] [nvarchar](450) NOT NULL,
	[ReportId] [nvarchar](450) NULL,
	[Date] [datetime2](7) NOT NULL,
	[StatusReport] [int] NULL,
	[AdminStatusReport] [int] NULL,
	[ApplicationUserId] [nvarchar](450) NULL,
 CONSTRAINT [PK_LogReports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organizations](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsHolding] [bit] NOT NULL,
	[Region] [int] NOT NULL,
	[IsState] [bit] NOT NULL,
	[TypeActivityId] [nvarchar](450) NULL,
	[UNP] [nvarchar](max) NULL,
	[TypeEconomicActivity] [nvarchar](max) NULL,
	[OrganizationalLegalForm] [nvarchar](max) NULL,
	[GovermentForReport] [nvarchar](max) NULL,
	[UnitForReport] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Position1] [nvarchar](max) NULL,
	[FullName1] [nvarchar](max) NULL,
	[Position2] [nvarchar](max) NULL,
	[FullName2] [nvarchar](max) NULL,
 CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PeriodReportTypes]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeriodReportTypes](
	[PeriodId] [nvarchar](450) NOT NULL,
	[ReportTypeId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_PeriodReportTypes] PRIMARY KEY CLUSTERED 
(
	[PeriodId] ASC,
	[ReportTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Periods]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Periods](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_Periods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[Id] [nvarchar](450) NOT NULL,
	[FormCollectionId] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NULL,
	[Date] [datetime2](7) NULL,
	[ReplyDate] [datetime2](7) NULL,
	[StatusReport] [int] NULL,
	[AdminStatusReport] [int] NULL,
	[ReportTypeId] [nvarchar](450) NULL,
	[Description] [nvarchar](max) NULL,
	[UserCheckinIntervalId] [nvarchar](450) NULL,
	[AttachmentFileId] [nvarchar](450) NULL,
	[Note] [nvarchar](max) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ReturnDate] [datetime2](7) NULL,
	[IsRead] [bit] NOT NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportTypes]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportTypes](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReportTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tabs]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tabs](
	[Id] [nvarchar](450) NOT NULL,
	[CollectionId] [nvarchar](max) NULL,
	[Json] [nvarchar](max) NULL,
	[NoValid] [nvarchar](max) NULL,
	[TabId] [int] NOT NULL,
 CONSTRAINT [PK_Tabs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TypeActivities]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeActivities](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IsIndustrial] [bit] NOT NULL,
	[Position] [int] NOT NULL,
 CONSTRAINT [PK_TypeActivities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserCheckinIntervals]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCheckinIntervals](
	[Id] [nvarchar](450) NOT NULL,
	[OrganizationId] [nvarchar](450) NULL,
	[Year] [int] NOT NULL,
	[PeriodId] [nvarchar](450) NULL,
	[Date] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserCheckinIntervals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 31.05.2021 20:13:44 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUsers_OrganizationId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_OrganizationId] ON [dbo].[AspNetUsers]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 31.05.2021 20:13:44 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LogReports_ApplicationUserId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_LogReports_ApplicationUserId] ON [dbo].[LogReports]
(
	[ApplicationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_LogReports_ReportId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_LogReports_ReportId] ON [dbo].[LogReports]
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Organizations_TypeActivityId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_Organizations_TypeActivityId] ON [dbo].[Organizations]
(
	[TypeActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PeriodReportTypes_ReportTypeId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_PeriodReportTypes_ReportTypeId] ON [dbo].[PeriodReportTypes]
(
	[ReportTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reports_AttachmentFileId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_AttachmentFileId] ON [dbo].[Reports]
(
	[AttachmentFileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reports_ReportTypeId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_ReportTypeId] ON [dbo].[Reports]
(
	[ReportTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reports_UserCheckinIntervalId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_UserCheckinIntervalId] ON [dbo].[Reports]
(
	[UserCheckinIntervalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reports_UserId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_UserId] ON [dbo].[Reports]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserCheckinIntervals_OrganizationId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_UserCheckinIntervals_OrganizationId] ON [dbo].[UserCheckinIntervals]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserCheckinIntervals_PeriodId]    Script Date: 31.05.2021 20:13:44 ******/
CREATE NONCLUSTERED INDEX [IX_UserCheckinIntervals_PeriodId] ON [dbo].[UserCheckinIntervals]
(
	[PeriodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[LogReports]  WITH CHECK ADD  CONSTRAINT [FK_LogReports_AspNetUsers_ApplicationUserId] FOREIGN KEY([ApplicationUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[LogReports] CHECK CONSTRAINT [FK_LogReports_AspNetUsers_ApplicationUserId]
GO
ALTER TABLE [dbo].[LogReports]  WITH CHECK ADD  CONSTRAINT [FK_LogReports_Reports_ReportId] FOREIGN KEY([ReportId])
REFERENCES [dbo].[Reports] ([Id])
GO
ALTER TABLE [dbo].[LogReports] CHECK CONSTRAINT [FK_LogReports_Reports_ReportId]
GO
ALTER TABLE [dbo].[Organizations]  WITH CHECK ADD  CONSTRAINT [FK_Organizations_TypeActivities_TypeActivityId] FOREIGN KEY([TypeActivityId])
REFERENCES [dbo].[TypeActivities] ([Id])
GO
ALTER TABLE [dbo].[Organizations] CHECK CONSTRAINT [FK_Organizations_TypeActivities_TypeActivityId]
GO
ALTER TABLE [dbo].[PeriodReportTypes]  WITH CHECK ADD  CONSTRAINT [FK_PeriodReportTypes_Periods_PeriodId] FOREIGN KEY([PeriodId])
REFERENCES [dbo].[Periods] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PeriodReportTypes] CHECK CONSTRAINT [FK_PeriodReportTypes_Periods_PeriodId]
GO
ALTER TABLE [dbo].[PeriodReportTypes]  WITH CHECK ADD  CONSTRAINT [FK_PeriodReportTypes_ReportTypes_ReportTypeId] FOREIGN KEY([ReportTypeId])
REFERENCES [dbo].[ReportTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PeriodReportTypes] CHECK CONSTRAINT [FK_PeriodReportTypes_ReportTypes_ReportTypeId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Files_AttachmentFileId] FOREIGN KEY([AttachmentFileId])
REFERENCES [dbo].[Files] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Files_AttachmentFileId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_ReportTypes_ReportTypeId] FOREIGN KEY([ReportTypeId])
REFERENCES [dbo].[ReportTypes] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_ReportTypes_ReportTypeId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_UserCheckinIntervals_UserCheckinIntervalId] FOREIGN KEY([UserCheckinIntervalId])
REFERENCES [dbo].[UserCheckinIntervals] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_UserCheckinIntervals_UserCheckinIntervalId]
GO
ALTER TABLE [dbo].[UserCheckinIntervals]  WITH CHECK ADD  CONSTRAINT [FK_UserCheckinIntervals_Organizations_OrganizationId] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[UserCheckinIntervals] CHECK CONSTRAINT [FK_UserCheckinIntervals_Organizations_OrganizationId]
GO
ALTER TABLE [dbo].[UserCheckinIntervals]  WITH CHECK ADD  CONSTRAINT [FK_UserCheckinIntervals_Periods_PeriodId] FOREIGN KEY([PeriodId])
REFERENCES [dbo].[Periods] ([Id])
GO
ALTER TABLE [dbo].[UserCheckinIntervals] CHECK CONSTRAINT [FK_UserCheckinIntervals_Periods_PeriodId]
GO
/****** Object:  StoredProcedure [dbo].[InsertPeriods]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertPeriods]
	@Name NVARCHAR(max)
AS
BEGIN
	INSERT INTO [dbo].[Periods] (Id,Name)
		values (NEWID(),@Name);
END
GO
/****** Object:  StoredProcedure [dbo].[InsertRole]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertRole]
	@Description NVARCHAR(max),
	@Name NVARCHAR(256),
	@NormalizedName NVARCHAR(256),
	@ConcurrencyStamp NVARCHAR(max)
AS
BEGIN
	INSERT INTO [dbo].[AspNetRoles] (Id,Description,Name,NormalizedName,ConcurrencyStamp)
		values (NEWID(),@Description,
				@Name,@NormalizedName,@ConcurrencyStamp);
END
GO
/****** Object:  StoredProcedure [dbo].[InsertUser]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertUser]
	@OrganizationId NVARCHAR(450) = null,
	@PasswordEncrypt nvarchar(MAX) = null,
	@Fio nvarchar(MAX) = null,
	@UserName NVARCHAR(256)= null,
	@Email nvarchar(256)= null,
	@EmailConfirmed bit,
	@PasswordHash nvarchar(MAX)= null,
	@SecurityStamp NVARCHAR(max)= null,
	@ConcurrencyStamp nvarchar(MAX)= null,
	@PhoneNumber nvarchar(MAX)= null,
	@PhoneNumberConfirmed bit,
	@TwoFactorEnabled bit,
	@LockoutEnd datetimeoffset(7)= null,
	@LockoutEnabled bit,
	@AccessFailedCount int
AS
BEGIN
	INSERT INTO [dbo].[AspNetUsers] (Id,OrganizationId,PasswordEncrypt,Fio,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount)
		values (NEWID(),@OrganizationId,@PasswordEncrypt,@Fio,dbo.Transalater(@UserName),UPPER(dbo.Transalater(@UserName)),@Email,UPPER(@Email),@EmailConfirmed,@PasswordHash,@SecurityStamp,@PhoneNumber,@PhoneNumberConfirmed,@TwoFactorEnabled,@LockoutEnd,@LockoutEnabled,@AccessFailedCount);
END
GO
/****** Object:  StoredProcedure [dbo].[SetHistory]    Script Date: 31.05.2021 20:13:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetHistory]
	-- Add the parameters for the stored procedure here
	@OldData NVARCHAR(max),
	@NewData NVARCHAR(max),
	@TableName NVARCHAR(100)
AS
BEGIN
	INSERT INTO [dbo].[DataHistory] (Id,OldData,NewData,TableName,HistoryDate)
			values (NEWID(),@OldData,
					@NewData,@TableName,GETDATE());
END
GO
USE [master]
GO
ALTER DATABASE [forest-report] SET  READ_WRITE 
GO
