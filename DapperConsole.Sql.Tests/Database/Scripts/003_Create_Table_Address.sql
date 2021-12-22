
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AddressLine] [nvarchar](60) NOT NULL,
	[City] [nvarchar](30) NOT NULL,
	[StateProvinceId] [int] NOT NULL,
	[PostalCode] [nvarchar](15) NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Address_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF_Address_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]

ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_StateProvince_Id] FOREIGN KEY([StateProvinceId])
REFERENCES [dbo].[StateProvince] ([Id])

ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_StateProvince_Id]