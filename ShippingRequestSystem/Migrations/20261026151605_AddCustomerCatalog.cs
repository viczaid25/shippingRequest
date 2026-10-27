using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prcPurchaseCustomerCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Neighborhood = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseCustomerCatalog", x => x.Id);
                });

            // Carga inicial: catálogo de clientes 2026 de Trade (Excel), normalizado.
            var now = new DateTime(2026, 10, 26, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "prcPurchaseCustomerCatalog",
                columns: new[] { "CustomerCode", "BusinessName", "TaxId", "PostalCode", "Street",
                                 "Neighborhood", "City", "State", "Country", "Currency", "IsActive", "CreatedAt" },
                values: new object?[,]
                {
                    { "ACEWAY", "ACEWAY DE MEXICO", "AME150305BI5", "76246", "Carretera Estatal 431, Interior 75", "Hacienda la Machorra", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "USD", true, now },
                    { "ACTRON", "ACTRON TECHNOLOGY CORPORATION", "1674810", "33855", "1F, No. 22, Sec. 2, Nan-Kan Rd.", null, "Taoyuan", "Taoyuan", "Taiw\u00e1n", "USD", true, now },
                    { "ADVANEX", "ADVANEX DE MEXICO", "AME171006GT1", "76295", "Carretera Estatal 100 Km. 2.565", "Parque Industrial FINSA II", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "USD", true, now },
                    { "AER", "AER TECHNOLOGIES, INC", "95-2043956", "92821", "650 Columbia Street", null, "Brea", "California", "Estados Unidos", "USD", true, now },
                    { "ASTEMO S", "ASTEMO SILAO", "SAM1211014G9", "36275", "MINERAL DE PE\u00d1AFIEL No. 77", "Parque Industrial Santa Fe IV Puerto Interior", "Silao", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "CHANGZHOU", "CHANGZHOU SHIHLIN AUTO PARTS CO.,LTD", "91320411067680880D", "213031", "No. 7 Xin4 Rd.", "New District", "Changzhou", "Jiangsu", "China", "USD", true, now },
                    { "CIMEF", "COMERCIALIZADORA DE INSUMOS METALICOS PARA FUNDICION", "CIM150211T15", "76265", "Carretera 500 Chichimequillas - La Griega Km. 18.5", "Amazcala", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "MXN", true, now },
                    { "CRESC", "CRESCENT ELECTRONICS", "101202034RT0001", "N8X 4G1", "3115 Marentette Avenue", null, "Windsor", "Ontario", "Canad\u00e1", "USD", true, now },
                    { "FCA LATAM", "FCA FIAT CHRYSLER AUTOMOVEIS BRASIL LTDA", "16.701.716/0001-56", "32669-900", "AVENIDA CONTORNO 3455", "BAIRRO PAULO CAMILO", "Betim", "Minas Gerais", "Brasil", "USD", true, now },
                    { "STELLANTIS", "FCA US LLC", "27-0187277", "48326-2766", "1000 CHRYSLER DRIVE, CIMS:485-12-30", "-", "Auburn Hills", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD SP", "FORD MOTOR COMPANY", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "JPY", true, now },
                    { "FORD HN7EA", "FORD MOTOR COMPANY S.A. DE C.V.", "FMO8304236C5", "53126", "HENRY FORD No. 100 - PISO 5", "SAN MATEO NOPALA", "Naucalpan De Ju\u00e1rez", "Estado de M\u00e9xico", "M\u00e9xico", "USD", true, now },
                    { "FORD M5 CE", "FORD MOTOR COMPANY", "380549190", "44142", "Brookpark Rd", "Brookpark", "Brookpark", "Ohio", "Estados Unidos", "JPY", true, now },
                    { "FORD DRA", "FORD MOTOR COMPANY", "FMO8304236C5", "53126", "HENRY FORD No. 100 - PISO 5", "SAN MATEO NOPALA", "Naucalpan De Ju\u00e1rez", "Estado de M\u00e9xico", "M\u00e9xico", "JPY", true, now },
                    { "FORD EUR", "FORD MOTOR COMPANY", "DE122653611", "50769", "Edsel Ford Strasse 2-14", null, "Koeln", "Chorweiler", "Alemania", "MXN", true, now },
                    { "FORD", "FORD MOTOR COMPANY", "FMO-830423-6C5", "53126", "HENRY FORD No. 100 - PISO 5", "SAN MATEO NOPALA", "Naucalpan De Ju\u00e1rez", "Estado de M\u00e9xico", "M\u00e9xico", "MXN", true, now },
                    { "FORD SP HN", "FORD MOTOR COMPANY", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD DEP", "FORD MOTOR COMPANY", "380549190", "48120", "Miller Rd (Miller Road)", "Dearborn", "Dearborn", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD CEP", "FORD MOTOR COMPANY", "380549190", "44142", "Brookpark Rd", "Brookpark", "Brookpark", "Ohio", "Estados Unidos", "USD", true, now },
                    { "FORD VEP", "FORD MOTOR COMPANY", "B46066361", "46440", "Pol\u00edgono Industrial", "Almussafes", "Almussafes", "Valencia", "Espa\u00f1a", "USD", true, now },
                    { "FORD M5 DE", "FORD MOTOR COMPANY", "380549190", "48120", "Miller Rd (Miller Road)", "Dearborn", "Dearborn", "Michigan", "Estados Unidos", "JPY", true, now },
                    { "FORD SP HN7EA", "FORD MOTOR COMPANY", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "U7SAK SP", "FORD MOTOR COMPANY SP", "XEXX010101000", "48151", "11872 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD EUR", "FORD MOTOR COMPANY SP", "DE122653611", "50769", "Edsel Ford Strasse 2-14", null, "Koeln", "Chorweiler", "Alemania", "MXN", true, now },
                    { "FORD SP", "FORD MOTOR COMPANY SP", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD SP HN", "FORD MOTOR COMPANY SP", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "FORD EUR", "FORD WERKE GMBH", "DE122653611", "50769", "Edsel-Ford-Stra\u00dfe 2-14", null, "Koeln", "Chorweiler", "Alemania", "USD", true, now },
                    { "FORD SP HN", "FORD WERKE GMBH", "XEXX010101000", "48151", "11871 MIDDLEBELT RD", null, "Livonia", "Michigan", "Estados Unidos", "USD", true, now },
                    { "GMNA", "GENERAL MOTORS SPRING HILL", "27-0756180", "37174", "Saturn Parkway", "Spring Hill", "Spring Hill", "Tennessee", "Estados Unidos", "USD", true, now },
                    { "SMART", "GRUPO TECNOLOGICO SMART", "GTS071009C61", "72570", "RIO NAZAS No. 5308", "JARDINES DE SAN MANUEL", "Puebla", "Puebla", "M\u00e9xico", "USD", true, now },
                    { "SHOWA", "HITACHI ASTEMO SILAO", "SAM1211014G9", "36275", "Mineral de Pe\u00f1afiel No. 77", "Puerto Interior", "Silao", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "HDM CELAYA", "HONDA DE MEXICO", "HME850910RF1", "45680", "CARRETERA A EL CASTILLO No. 7250", "PARQUE INDUSTRIAL", "El Salto", "Jalisco", "M\u00e9xico", "USD", true, now },
                    { "HDM", "HONDA DE MEXICO", "HME850910RF1", "45680", "CARRETERA A EL CASTILLO No. 7250", "PARQUE INDUSTRIAL", "El Salto", "Jalisco", "M\u00e9xico", "USD", true, now },
                    { "HDMA", "HONDA DEV MFG OF AMERICA", "20-0374964/21083760", "43040", "24000 Honda Parkway", null, "Marysville", "Ohio", "Estados Unidos", "USD", true, now },
                    { "55933X1", "INTERNATIONAL MOTORS MEXICO CV", "CMI950920TR8", "11510", "Blvd. Manuel \u00c1vila Camacho 201, Piso 5", "Polanco I Secci\u00f3n", "Miguel Hidalgo", "Ciudad de M\u00e9xico", "M\u00e9xico", "USD", true, now },
                    { "55933X1 US", "INTERNATIONAL MOTORS, LLC", "36-1264810", "60532", "International Drive 2701", null, "Lisle", "Illinois", "Estados Unidos", "USD", true, now },
                    { "JFEMEX", "JFE SHOJI STEEL DE MEXICO", "JSS981210P91", "22210", "Paseo del Cucapah No. 10515", "El Lago (Grandes Lagos)", "Tijuana", "Baja California", "M\u00e9xico", "USD", true, now },
                    { "JTEKT", "JTEKT AUTOMOTIVE MEXICO", "JAM140218UQ0", "78423", "AV MONTECILLO No. 100", "CIUDAD SATELITE", "San Luis Potosi", "San Luis Potos\u00ed", "M\u00e9xico", "USD", true, now },
                    { "JATV", "JTEKT AUTOMOTIVE NORTH AMERICA, INC.", "22-2896829", "29607", "7 RESEARCH DRIVE", "-", "Greenville", "South Carolina", "Estados Unidos", "USD", true, now },
                    { "KATOLEC", "KATOLEC DE GUANAJUATO", "KGU-141013-BU8", "36987", "AVE, MIGUEL HIDALGO", "Marabis Abasolo Industrial Park", "-", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "MAZDA MC", "MAZDA MOTOR CORPORATION", "3240001036223", "735-0028", "3-1 SHINCHI, FUCHU-CHO, AKI-GUN", "-", "Hiroshima", "Hiroshima", "Jap\u00f3n", "USD", true, now },
                    { "MMVO", "MAZDA MOTOR MANUFACTURING DE MEXICO", "MMM110613MZ4", "76875", "AV. HIROSHIMA No. 1000", "COMPLEJO INDUSTRIAL SALAMANCA", "Salamanca", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "MMA", "MAZDA MOTOR OF AMERICA, INC", "95-2668359", "92618", "200 Spectrum Center Drive, Suite 100", null, "Irvine", "California", "Estados Unidos", "USD", true, now },
                    { "MELX", "MELCO LOGISTICS DE MEXICO", "MLM140204PI7", "76245", "CARRETERA ESTATAL 500, KM4+300 No. LOTE 6", "PARQUE INDUSTRIAL O DONNELL AEROPUERTO", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "MXN", true, now },
                    { "MERCEDES", "MERCEDES-BENZ MEXICO INTERNATIONAL", "DCM150604MG0", "05348", "AVENIDA SANTA FE No. 428 - TORRE 3, PISO 3", "Santa Fe, Cuajimalpa", "Cuajimalpa De Morelos", "Ciudad de M\u00e9xico", "M\u00e9xico", "USD", true, now },
                    { "MJEM", "MEXICO JINYOUNG TECHNO", "MJT1508318K7", "76806", "Av. Aztecas No. 23", "Parque Industrial Nuevo San Juan", "San Juan del R\u00edo", "Quer\u00e9taro", "M\u00e9xico", "USD", true, now },
                    { "MEAP", "MITSUBISHI ELECTRIC ASIA PTE. LTD.", "159943", "159943", "307 ALEXANDRA ROAD", null, "Singapore", "Singapore", "Estados Unidos", "USD", true, now },
                    { "MIT3700", "MITSUBISHI ELECTRIC AUTOMOTIVE AMERICA INC", "31-1208705", "48168", "Centennial Drive", "Northville", "Northville", "Michigan", "Estados Unidos", "USD", true, now },
                    { "MEAX", "MITSUBISHI ELECTRIC AUTOMOTIVE DE MEXICO", "MEA130306VE1", "76245", "CARRETERA ESTATAL 500 KM 4+300, LOTE 6", "PARQUE INDUSTRIAL O DONNELL AEROPUERTO", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "USD", true, now },
                    { "MELAC", "MITSUBISHI ELECTRIC AUTOMOTIVE(CHINA)CO.,LTD", "91320581576709602U", "215500", "NO.88 Lushan Road, New&Hi-tech", "Industrial Development Zone", "Changshu City", "Jiangsu", "China", "USD", true, now },
                    { "MBD3219", "MITSUBISHI ELECTRIC CORPORATION", "4010001008772", "670-8677", "840, Chiyodamachi", null, "Himeji-shi", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "MTHIM", "MITSUBISHI ELECTRIC CORPORATION", "4010001008772", "670-8677", "671, FUJICHO, HIROHATAKU", null, "Hyogo", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "MBD3219H", "MITSUBISHI ELECTRIC CORPORATION HIMEJI WORKS", "4010001008772", "670-8677", "840, Chiyodamachi", null, "Himeji-shi", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "SANDA", "MITSUBISHI ELECTRIC CORPORATION SANDA WORKS", "4010001008772", "669-1513", "2-3-33 Miwa", null, "Sanda city", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "MBD3219S", "MITSUBISHI ELECTRIC CORPORATION SANDA WORKS", "4010001008772", "669-1513", "2-3-33 Miwa", null, "Sanda city", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "MHK2019", "MITSUBISHI ELECTRIC HONG KONG LIMITED", "06/02009640", "UNIT 20-22, 5/F", "Citipak Pacific Pty Ltd.", "SHEUNG, SHUI PLAZA", "Sheung Shui", "Hong Kong", "Jap\u00f3n", "USD", true, now },
                    { "MBD3219H", "MITSUBISHI ELECTRIC MOBILITY CORPORATION", "2010001239816", "100-8310", "MARUNOUCHI 2-7-3", null, "Marunouchi 2-7-3", "Tokyo", "Jap\u00f3n", "USD", true, now },
                    { "MBD3219SS", "MITSUBISHI ELECTRIC MOBILITY CORPORATION", "2010001239816", "100-8310", "MARUNOUCHI 2-7-3", null, "Marunouchi 2-7-3", "Tokyo", "Jap\u00f3n", "USD", true, now },
                    { "META", "MITSUBISHI ELECTRIC THAI AUTO-PARTS CO., LTD", "105539126458", "21140", "Siam Eastern Industrial Park, 60/16 Moo3,", "Mabyangporn", "Pluakdang", "Rayong", "Tailandia", "USD", true, now },
                    { "MBD3219T", "MITSUBISHI ELECTRIC TRADING (THAILAND) CO., LTD.", "105541077779", "10260", "No. 1, MD Tower, 21st Floor, Room C3, E, F", "Bang Na Nuea Sub-district", "Bang Na District", "Bangkok", "Tailandia", "USD", true, now },
                    { "MRK0215", "MITSUBISHI ELECTRIC TRADING CORPORATION", "2010001239816", "100-8310", "MARUNOUCHI 2-7-3", null, null, "Tokyo", "Jap\u00f3n", "USD", true, now },
                    { "MRK0219", "MITSUBISHI ELECTRIC TRADING CORPORATION SANDA", "1010001095657", "669-1513", "2-3-33 Miwa", null, "Sanda city", "Hyogo", "Jap\u00f3n", "USD", true, now },
                    { "KRY12", "MITSUBISHI ELECTRIC&ELECTRONICS(SHANGHAI)CO.,LTD.", "9.13100006073732e+17", "200336", "29/F.,Shanghai Maxdo Center, No.8", "XingYi Road", "Chang Ning", "Shanghai", "Jap\u00f3n", "USD", true, now },
                    { "NEDEC", "NEDEC MEXICO", "NME140127PD3", "37980", "Av. Antonio Guti\u00e9rrez Cortina No. 18", "Parque Opci\u00f3n", "San Jos\u00e9 Iturbide", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "NMEX", "NISSAN MEXICANA", "NME-610911-L71", "01090", "Av. Insurgentes Sur", "Barrio Loreto", "Ciudad de M\u00e9xico", "Ciudad de M\u00e9xico", "M\u00e9xico", "MXN", true, now },
                    { "NNA", "NISSAN NORTH AMERICA", "210801000", "39046", "554 Nissan Way", "Canton", "Canton", "Mississippi", "Estados Unidos", "USD", true, now },
                    { "NISSAN SPRING", "NISSAN SPRING (THAILAND) CO. LTD.", "105548037420", "21140", "500/10 Moo 3", "T. Tasit", "Pluak Daeng", "Rayong", "Tailandia", "USD", true, now },
                    { "PEGASUS", "PEGASUS-SHIMAMOTO AUTO PARTS (VIETNAM) CO., LTD", "3603045422", "76000", "Long Duc Industrial Park", "Long Duc Ward", "Long Thanh District", "Dong Nai Province", "Vietnam", "USD", true, now },
                    { "JONES", "R. L. JONES SAN DIEGO INC.", "33-0794393", "92154", "8830 Siempre Viva Rd., Suite 100", null, "San Diego", "California", "Estados Unidos", "USD", true, now },
                    { "REA", "REA MAGNET WIRE COMPANY INC", "35-0602855", "47909", "2800 Concord Rd", null, "Lafayette", "Indiana", "Estados Unidos", "USD", true, now },
                    { "REXME", "RECICLAJE Y EXPORTACION DE METALES REXME", "REM160824HIA", "76246", "LA PIEDAD", "EL COLORADO", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "MXN", true, now },
                    { "SANKO", "SANKO GOSEI MEXICO", "SGM120703Q44", "38150", "FERROPUERTO No. 205", "RINCON DE TAMAYO", "Celaya", "Guanajuato", "M\u00e9xico", "USD", true, now },
                    { "SENJU", "SENJU COMTEK CORP", "911832930", "95051", "2989 SAN YSIDRO WAY", null, "Santa Clara", "California", "Estados Unidos", "USD", true, now },
                    { "SHENZHEN", "SHENZHEN KAIZHONG PRECISION TECHNOLOGY CO., LTD.", "91440300682020833Q", "518118", "No. 1, Planning 4th Road, Longtian Street, Pingshan District", null, "Shenzhen", "Guangdong", "China", "USD", true, now },
                    { "STANLEY", "STANLEY ELECTRIC MANUFACTURING MEXICO", "SEM1504074Y0", "47515", "Av. Rita P\u00e9rez de Moreno 2045", "Parque Industrial Colinas de los Lagos", "Lagos de Moreno", "Jalisco", "M\u00e9xico", "USD", true, now },
                    { "JV SME", "STANLEY MOBILITY ELECTRIC MEXICO", "SME250711J25", "76245", "CARRETERA ESTATAL 500 KM 4+300, LOTE 6", "PARQUE INDUSTRIAL O DONNELL AEROPUERTO", "El Marqu\u00e9s", "Quer\u00e9taro", "M\u00e9xico", "USD", true, now },
                    { "STLA LATAM", "STELLANTIS AUTOMOVEIS BRASIL LTDA", "16.701.716/0001-56", "32669-900", "AVENIDA CONTORNO 3455", "BAIRRO PAULO CAMILO", "Betim", "Minas Gerais", "Brasil", "USD", true, now },
                    { "FCA", "STELLANTIS MEXICO", "CME720930GM9", "05348", "AV.PROLONGACION PASEO DE LA REFORMA No. 1240", "SANTA FE CUAJIMALPA", "Cuajimalpa De Morelos", "Ciudad de M\u00e9xico", "M\u00e9xico", "USD", true, now },
                    { "SUMITRONICS", "SUMITRONICS USA INC.", "42-1724498", "92154", "9335 AIRWAY ROAD, SUITE 212", "-", "San Diego", "California", "Estados Unidos", "USD", true, now },
                    { "TAIYO", "TAIYO TECHNOLOGY INDUSTRY (THAILAND) CO.,LTD", "115555004761", "10570", "88/83 Moo. 15", null, "Bang Sao Thong", "Samut Prakan", "Tailandia", "USD", true, now },
                    { "TAYLOR", "TAYLOR LABELING SOLUTIONS", "IUN941111CQ3", "66645", "Av. Los Encinos 123", "El Sabinal Industrial Park", "Apodaca", "Nuevo Le\u00f3n", "M\u00e9xico", "USD", true, now },
                    { "TERADAI", "TERADAI MEXICO", "TME141121759", "76974", "Av. Industria Aeron\u00e1utica No. Lote 2 - Manzana 4", "San Antonio La Galera", "Huimilpan", "Quer\u00e9taro", "M\u00e9xico", "MXN", true, now },
                    { "TOKYOE", "TOKYO ELECTRON DEVICE AMERICA,INC.", "46-3726055", "94539", "927 Corporate Way", null, "Fremont", "California", "Estados Unidos", "USD", true, now },
                    { "TOPY", "TOPY FASTENERS MEXICO", "TFM150701VE7", "11560", "ANDRES BELLO No. 45 - PISO 24", "POLANCO V SECCION", "Ciudad de M\u00e9xico", "Ciudad de M\u00e9xico", "M\u00e9xico", "USD", true, now },
                    { "UNITED RADIO", "UNITED RADIO, INC", "16-0906884", "13057", "5705 Enterprise Parkway", null, "East Syracuse", "New York", "Estados Unidos", "USD", true, now },
                    { "VAMM", "VICTOR ALBERTO MOCTEZUMA MORAN", "MOMV841215DT2", "76295", "HACIENDA AGUA AZUL #2", "GALERAS", "Col\u00f3n", "Quer\u00e9taro", "M\u00e9xico", "MXN", true, now }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchaseCustomerCatalog");
        }
    }
}
