//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlTools.ServiceLayer.Scripting.Contracts;
using Microsoft.SqlTools.ServiceLayer.Test.Common;
using NUnit.Framework;

namespace Microsoft.SqlTools.ServiceLayer.TestDriver.Tests
{
    [TestFixture]
    /// <summary>
    /// Scripting service end-to-end integration tests that use the SqlScriptPublishModel type to generate scripts.
    /// </summary>
    public class SqlScriptPublishModelTests 
    {
        [OneTimeSetUp]
        public void SetupSqlScriptPublishModelTests()
        {
            Fixture = new ScriptingFixture();
        }

        private static ScriptingFixture Fixture { get; set; }

        private SqlTestDb Northwind { get { return Fixture.Database; } }

        [Test]
        public async Task ListSchemaObjects()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            using (SelfCleaningTempFile tempFile = new SelfCleaningTempFile())
            {
                ScriptingListObjectsParams requestParams = new ScriptingListObjectsParams
                {
                    ConnectionString = this.Northwind.ConnectionString,
                };

                ScriptingListObjectsResult result = await testService.ListScriptingObjects(requestParams);
                ScriptingListObjectsCompleteParams completeParameters = await testService.Driver.WaitForEvent(ScriptingListObjectsCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.AreEqual(ScriptingFixture.ObjectCountWithoutDatabase, completeParameters.ScriptingObjects.Count);
            }
        }

        [Test]
        public async Task ScriptDatabaseSchema()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaOnly",
                    },
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.True(parameters.Success);
            }
        }

        [Test]
        public async Task ScriptDatabaseSchemaAndData()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaAndData",
                    },
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingCompleteParams completeParameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.True(completeParameters.Success);
            }
        }

        [Test]
        public async Task ScriptTable()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaOnly",

                    },
                    ScriptingObjects = new List<ScriptingObject>
                    {
                        new ScriptingObject
                        {
                            Type = "Table",
                            Schema = "dbo",
                            Name = "Customers",
                        },
                    }
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingPlanNotificationParams planEvent = await testService.Driver.WaitForEvent(ScriptingPlanNotificationEvent.Type, TimeSpan.FromSeconds(1));
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.True(parameters.Success);
                Assert.AreEqual(1, planEvent.Count);
            }
        }

        [Test]
        public async Task ScriptTableUsingIncludeFilter()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaOnly",
                    },
                    IncludeObjectCriteria = new List<ScriptingObject>
                    {
                        new ScriptingObject
                        {
                            Type = "Table",
                            Schema = "dbo",
                            Name = "Customers",
                        },
                    }
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingPlanNotificationParams planEvent = await testService.Driver.WaitForEvent(ScriptingPlanNotificationEvent.Type, TimeSpan.FromSeconds(30));
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.True(parameters.Success);
                // Work around SMO bug https://github.com/microsoft/sqlmanagementobjects/issues/19 which leads to non-unique URNs in the collection
                Assert.That(planEvent.Count, Is.AtLeast(1), "ScripingPlanNotificationParams.Count");
                Assert.That(planEvent.ScriptingObjects.All(obj => obj.Name == "Customers" && obj.Schema == "dbo" && obj.Type == "Table"), "ScriptingPlanNotificationParams.ScriptingObjects contents");
            }
        }

        [Test]
        public async Task ScriptTableAndData()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaAndData",
                    },
                    ScriptingObjects = new List<ScriptingObject>
                    {
                        new ScriptingObject
                        {
                            Type = "Table",
                            Schema = "dbo",
                            Name = "Customers",
                        },
                    }
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingPlanNotificationParams planEvent = await testService.Driver.WaitForEvent(ScriptingPlanNotificationEvent.Type, TimeSpan.FromSeconds(30));
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(30));
                Assert.True(parameters.Success);
                // Work around SMO bug https://github.com/microsoft/sqlmanagementobjects/issues/19 which leads to non-unique URNs in the collection
                Assert.That(planEvent.Count, Is.AtLeast(1), "ScripingPlanNotificationParams.Count");
                Assert.That(planEvent.ScriptingObjects.All(obj => obj.Name == "Customers" && obj.Schema == "dbo" && obj.Type == "Table"), "ScriptingPlanNotificationParams.ScriptingObjects contents");
            }
        }

        [Test]
        public async Task ScriptTableDoesNotExist()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaOnly",
                        ContinueScriptingOnError = false
                    },
                    ScriptingObjects = new List<ScriptingObject>
                    {
                        new ScriptingObject
                        {
                            Type = "Table",
                            Schema = "dbo",
                            Name = "TableDoesNotExist",
                        },
                    }
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(15));
                Assert.True(parameters.HasError);
                Assert.That(parameters.ErrorMessage, Contains.Substring("An error occurred while scripting the objects."), "parameters.ErrorMessage");
                Assert.That(parameters.ErrorDetails, Contains.Substring("The Table '[dbo].[TableDoesNotExist]' does not exist on the server."), "parameters.ErrorDetails");
            }
        }

        [Test]
        public async Task ScriptSchemaCancel()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = this.Northwind.ConnectionString,
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaAndData",
                    }
                };

                var result = Task.Run(() => testService.Script(requestParams));
                ScriptingProgressNotificationParams progressParams = await testService.Driver.WaitForEvent(ScriptingProgressNotificationEvent.Type, TimeSpan.FromSeconds(60));
                await Task.Run(() => testService.CancelScript(progressParams.OperationId));
                ScriptingCompleteParams cancelEvent = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(10));
                Assert.True(cancelEvent.Canceled);
            }
        }


        [Test]
        public async Task ScriptSchemaInvalidConnectionString()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    ConnectionString = "I'm an invalid connection string",
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaAndData",
                    },
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(10));
                Assert.True(parameters.HasError);
                Assert.AreEqual("Error parsing ScriptingParams.ConnectionString property.", parameters.ErrorMessage);
            }
        }

        [Test]
        public async Task ScriptSchemaInvalidFilePath()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            {
                ScriptingParams requestParams = new ScriptingParams
                {
                    FilePath = "This path doesn't even exist",
                    ConnectionString = "Server=Temp;Database=Temp;User Id=Temp;",
                    ScriptOptions = new ScriptOptions
                    {
                        TypeOfDataToScript = "SchemaAndData",
                    },
                };

                ScriptingResult result = await testService.Script(requestParams);
                ScriptingCompleteParams parameters = await testService.Driver.WaitForEvent(ScriptingCompleteEvent.Type, TimeSpan.FromSeconds(10));
                Assert.True(parameters.HasError);
                Assert.AreEqual("Invalid directory specified by the ScriptingParams.FilePath property.", parameters.ErrorMessage);
            }
        }

        [Test]
        public async Task ScriptSelectTable()
        {
            using (TestServiceDriverProvider testService = new TestServiceDriverProvider())
            using (SelfCleaningTempFile tempFile = new SelfCleaningTempFile())
            {
                await testService.Connect(tempFile.FilePath, testService.TestConnectionService.GetConnectionParameters(serverType: TestServerType.OnPrem));
                ScriptingParams requestParams = new ScriptingParams
                {
                    ScriptDestination = "ToEditor",
                    OwnerUri = tempFile.FilePath,
                    ScriptOptions = new ScriptOptions
                    {
                    },
                    ScriptingObjects = new List<ScriptingObject>
                    {
                        new ScriptingObject
                        {
                            Type = "Table",
                            Schema = "dbo",
                            Name = "Customers",
                        }
                    },
                    Operation = ScriptingOperationType.Select
                };
                ScriptingResult result = await testService.Script(requestParams);
                Assert.True(result.Script.Contains("SELECT"));
            }
        }

        private static void AssertSchemaInFile(string filePath, bool assert = true)
        {
            AssertFileContainsString(
                filePath,
                "CREATE DATABASE",
                assert);

            AssertFileContainsString(
                filePath,
                "create view [dbo].[Invoices] AS",
                assert);

            AssertFileContainsString(
                filePath,
                "CREATE TABLE [dbo].[Products](",
                assert);
        }

        private static void AssertTableDataInFile(string filePath, bool assert = true)
        {
            AssertFileContainsString(
                filePath,
                "INSERT [dbo].[Categories] ([CategoryID], [CategoryName], [Description], [Picture]) VALUES (1, N'Beverages', N'Soft drinks, coffees, teas, beers, and ales', 0x151C2F00020000000D000E0014002100FFFFFFFF4269746D617020496D616765005061696E742E5069637475726500010500000200000007000000504272757368000000000000000000A0290000424D98290000000000005600000028000000AC00000078000000010004000000000000000000880B0000880B00000800000008000000FFFFFF0000FFFF00FF00FF000000FF00FFFF000000FF0000FF00000000000000000010000000001000000001001000000000000001010100010000100100000001010001000000100100000100000010101000100000100010000000000000000000001000000001000000000000001000000000000000000000001001001000000000000000000001001001000012507070100000001000000001000100100000010000001000000000100100100000001001010101010000000000000000000001001000100000101010101000000000000000000000000000000000000101020525363777777777777753530100101000100000000001010001001000100100100000000000100000000000000000100010001001010001000000010010000000000000100000000000000000000000000000001014343577777777777777777777777777770100120001010100102000000000000000000000000100100010010000000000100010000000000000000010010000001001001010100000000000000000000000000000001021617777777775757535353525777777777777770150120000100010101000000000000000001000000000000001001001000000000010010000010010010000101001001000000100000001000000000000000000000001417777737617376377775767771707752777777776340161210300000000010000000000010000000000000000000000000000000000000000100000000000100000000000010000100000000000000000000000000100503677577571705014000000101070717775777777775334101400010101010001010010101000000000000000000000000000001000000000000000000000000000000001010001000001000000000000000000000000010357353000000000000000000000000000171777777777470612101000000001000001000000010000000000000000000000000000000100010703010101210000000000000000000000000000000000000000000000101034350000000010653435777570477474700000107757777171000000101001000101000101010000100000000000000000001041477110131035750020040477301000100000000000000000000000000000000000000000010004014176577707000067777777776560404007371734361701400001241000001000000001000020400000200000101747767777000101073777577777775372100000000100000000000000100100000000010000010000037676476700004000577470416717477421405000434140343000565120510100301014002107171370701437777777775777777112103177777777777705757771010000000100000000000000010000100000101000470440440671007477776777777614060600742524001777777747646776752060250003021716737777777777777774747727574777001016777777777767173317703000101010000000010000000001000000000000420460200077427477767777777775677777746474656606767777665746776757151253507166737777733746777577777777572776771100517777777767776015713141030020001000000001001000000000010100067440044047761477767776706767777674765673470657505767375664746757777271252167717733771737776777677567476577577121103775777777776717027753121010101000010000000000100001010000010767060004776747776776777756776777777777042477676067777467474747676777565256531311771177376477777576757777747710110701777777767777401717340000000000100001000000000001000000101004776540666050777677657777677470774777776747664607777376747476777777677717173735377717737747777777777774774770011271077777767777763171735210121010100000000000000000000010000000300406767757676775077006767477774774777774747770476777656706746777657777777777777777777737667777476574774777771001031777777776767741371771000000000010000000000000000000000101005707442567656176006767004770476707700767770000477747734656446745677676777777777777777777375667777777777777777773100010777777777777771753521010101010000000000100010000000010007777712152503004670077774767427047247776577564700076737747670474277777777777777777367777777765777777777777434777750757775377767676770172773100000000001000000000000001000101007170701614204046007746040676167470774167743656777740077776067407465677677777777777757717777737476775716161243410303077777777577775210000011350001001001000000101000100000100002100171525675070074340670005004076700706570757777767770077744746466567777677777777777777777773776777610000000137775350317777773777737750701000101021001000100000000000010100010010300067777761650604065047604760746404776406705656776770077764750474747677777777777777777773733747777773011735777777777777777757777777777767412041001001000001000001000000010001000577744140000607406706767676776777776477756767777447700774076646764777567777777777777737373737764677747753527777776777777777776365735353513010300120301010000000000000000001000107000210006147767674646040404040066667767677775476777046644644044456776767777777777733737373776777776774244777377717712777165357577534242040010010010000010001000000100010000100300050000146664000000101030734352101100065677767077770047604774377676777767777777777373737333756477657075377100770770177776525271673001012101210301001030000000100100000001000005000060046160004000125343510110101000000000007740000047744733737377757677777777777373737377737656757777777373101676770777717775671773001010300000021021010000000000000100000000100077400000414021414000000000000000000000000000300000777777773737377677677777777777373737333735677677777377710177777717774705271767340300000010101000100000100000100100000001014005660000000737560600000000000000000000000000004730777773733373737777747677777777737337353761666777777737737017771677077353777574735310012101000000010010100000100000000010004300065400000000400141254140404000000000000000000037737776773777373733777677777777777677646746565756747777773773017017710765654352735770017010303031010010000000100001001010030704000660000000000000040000000000000000000000000007777514673373373737777777476777777777474644764666776777777772711031076117307374357477373010341050043030012100100010100100012500000047000000000046742000000000000000000000000077776677777377377373733737767777777777767645676507574777657610057121101731611574777637735105270125213010050210100001070210301650000000640000000006776406776464000040641434177777767667614737337337373777777767677777776564767474664667477761775271112116101002331211101052721016120140161034106010173075617770000570047400047400446000000467770504777767173573756767776767737737773737377776777777777776564746765477576777176700774656474731010011000001250165214716170121012011070777173777400063770040000760467600000000740760600777067777777676767676767337333373737377747677777777776767747424676747677157701677677676131331213131301371317310312161525053073077777777700047577700000006006760000640400006474046740777777777676767676737737777373777777767777777777674746767467477777743670175305325352527135335353170143414371617130131211777177777777001737770006760476677047064466400047640077747777777767676767673773373333737373776746777777765467674704747674765375610731773573752534737417017035303130101010030001427777777737770047777770047460704644064400004640067004767077777777767676767673377377773737777776777777777766565665670767767775077007563153347370731013213617034343434307031417121177773777777740257737700027447000064000000000640064006760777777776767767767773373333373737777476777777777746765674464747767763477027172753717175777757757357171717171717433616163777777735737400737773400460660046000000000004000600676747777777776766766767377377777373737777747677777776756567467746765777117100537153353773777777777777777777777777737757737573777773773772047777350000474044600440000000000040047774007777700667677677633333333333737777766767777777777667476564657476760600007353375373177777777777777777777777777777737777377733753777740007177770000664024640640000000000004646700477777007767667666777777777773737777777777777776777446467565676777535373525317137177177777777777777777777777777777775377773753771737700076737350000000474664665646644400400464000077700067677677773333333333373737776676777777777767777766767765677771713175217037173777777777777777777777777775375377173753777377773700057777004007477667764766767667467600000004770000767667666677777777777337777775677777771777772604000404067761171613131535353717777777777777777777737753777777777753773717735374700000500670446677777776777776777776561004661000006767767777333131101100777777666567777567704040505140777716536353147173135371777777777777777777577577777777777777777353753777371700000001776040404040404606076767776170000470000071101100100000000000110157177776777776470124100002530004777111301313017535371777777777771771737377773777377753773717353252165376164464265700400000000000004040040076774000440000777500750000000000000000017347766777746564100000000400300652513530753303170737777775777777777777737777777773777753757035353134317137313533000046440000004400000000053770000000000077343100000000000000000004135777775676176000700000004044213052153115353371357777377737737775777777573757777777353213503161617163521657257000006700060042400000005273710000000000007577000000000000000000531117777665447405244000000040031501313030721353537737775777577753737717737777777777777035343343131303103171317337130100000567000200000031756000000000000000077771012100101101131117133375466747465707047000071502161011531534353517753773737353777737777777777737537713503353170717173561343105307030525370047014161717433700000000000000000000101011770000006402737373767456467777777773065773510137343531317073737773775777773777577375735737577777343375377373673071316352731717173137000007737352713574000000000000000000000000464000000046733737373446647777777777740007373737110310343537171773565373537577177375737777777777773353737717175357727753717163737357770000071735371677700000000000000000000000000460004004676173737374745777777777777004631713112031213131317337177737777777377777777777777777777775377737777377371717353773571747737377617771677773570000000000000000000000000400400000000406337333464673777777777774007733373311001013135317177737775377377777777777777777777777737777573777377777736771773773716717535343373525773700000000000000000000000000000000000000037337374433373777777777700007740010313133173137337357753777777777777777777777777777777737737775375737373717367171653735727367374753737174000000000000000000004600000000000000000373733643773373777777777404073000000000012137331737377777777777777777777777777777777777577773737773757575735317273353531757535737377576300000000000000000000424400000004000040007373375733337333377777770000700000000000000000070477777777777777777777777777777777777737773757753757373737777775357273673373773535737357000000000000000000004406000000000404004037337333773737737377777700400000000000004006404043777777773757777777777777777777777777773773737773777777717371737357171752573473721777340000000000000000000006446400000000004004337337333373337337337777100004705340100016503777747717717757777777777777777777777777773757757773577173577775777577377773777373757777177700000000000660000640047674000000004000003737337373377337373737774040077760004000000044004737777777777777777777777777777777777773773773577377777377377377377537177535757373537710000000000004040004640604600000000000400073733737337333737373777700000047477420000000000435777777777777777777777777777777777777757777777777777777777777777777777737737377577777000000000004600000460064600000004000000000373373337337373737373777600000000000550043617777777777777777777777777777777777777777773777777773777777777777777777777777777777737737777000000000000000000000406400000004040000003373373737337373737373770040000000002777357777777777777777777777777777777777777775777777777773777573717775777777577377777777777777757340400000000000000040004064000000000000000073373373337333373737377750000000000057777777777777777777777777777777177775737577737777777735777773777773773775377377735735735375737737000000000000000044600406060000000000000046337337337373777373733777007460000000377777777777777777777777777777777737737777377777377777737371775353753753777777777777777777737717750000000000000000000000444404400400000000063733737337333337373377774067400000000777777777777777777777777375773777757777177177377735777777777377777777777777777777777777777777777704000000000000000000006000666066000000004433733337337377333377777700676004004407777777777777777777777777777757357375377777775777737777777777777777777777777777777777777777777772010000000000000000000040004404440000000000373737337337337377777777704600674660077777777777777777777777777737777777777773773773777777777777777377377777737777753777777777777777750040000000000000000000000460460000000000463733733733733737777777770047464067000777777777777777777777777777777777777777777777777777771737177777757377377753777777777737757773737000000000000000000000644640000460000000000073373733733733777777773750660760400017777777777777777777777777777777777777777777777777777777777373773777357173775377735737777377757777240000000000000000000606400000000000000000373373773733777777777737604746400406057777777777777777777777777777777777777777775775771733735377757177175737753737537777757777777777750100000000000000000046540000000000000000007337333333777777777771771066067674767677777777777777777777777777777777377777777377737777775737573737736373717375773777373737377777371200400000000000000000046000000000000000000073737373777777777777737700656476464617777777777777777777777777775757777777575757735773735371737357737575357635733577377577777773777775000040000000000000440646000040000000000000733733377777777777777137106606476400077777777777777775777757357777777757577377375777775737777577735737377371735773757073737175777777370000000000000000046764656546400000000000007733737777537777777777774474407467005077777777777775777757377735737717737377777737777371773737373773577535373437073737757577737353777700500000000000004676474266640000000000000047333777074747777777777776567642766027777757537775777371735777777577777777577777775377777777577577777737777577737757757373737777775777000000400000000067407604040000000000000000077777103716173777777737676665646470577757377775777375777777177377777777777357357777773737777777371735737773735753737377777773577377370004000000000000666424604040000000000000000777777007677477777777767676767474003577777777773777777777777777773773573777377773777777577773777777777771775773777757353753577357777770010000000000040406404000244000000000000000777370141477567777777762476767660067777777773777777737773777753777777777777777777777777773777777777375367377375357367767767737673477140240000000000000446400004660000040000000007737520077772757777770040047667767177777757777777777777577737777757777717753717717777777757753535357777775775777777535753735757177357005004000000000000000040400476440464000000007773401616575777777006440004764256777377375775375735737777777737737737773777777777773777777777777771771777777777777773775777377577773000000040000000000400000000000067400000000077771425777367777700400060006765377777377777377737777777735735777777777777777777777757777777777777777777777777777777777777377377353770070040000000000000400000404000040000000000077770525765777777004004040440065773775717377777777377777777777777777777777777777777777777777777777777773737371775377773775657527777500004000000000000000000442424400064000000000777724077576777700400600007000373757373775775375375737777777777777777777777777777777777777737777377373577575777777573575373733771737300700004000000000000004646440000672440000000777507567657775000444040644047777377777773777777777757777777777777777777777777777777757377771777375773737373737373773377753575377577400004000000000000000000400000040440640000004777407757777700404246044604375777757737777777777777777777773777777777777777777777777377775773575737175717175717571757253372734372773007000040000000000000000000004600464000000007772525677777004704064240124373777377577777777777777773773777777573577777777777757377737373777373777737367363727373735356171737177175000400000400000000000000004600000400000000047710477777700676006564640577777777777777777777737773777777577177777777777777777377735775775377757173717535357174352537737373717717730070040000000000000000000040046000000000000077777711357047600446500072777777777737777777377777777573573777777777777777777777737777377377177377757773777377737777343574356773737710060040400400000000000000000400000000000000771571715356770446002470757775773777777377757735735773777777777777777777777777735777377777777777777737573577177535357773777371747527710160000000040000000000000006000000000000007771353777767600056440042735373775377375773777777777777777777777777777777777777777777777777777757377773777377737777735777537577373717700104004000000000000000000440000000000000077171357777674006064214357577775737757777777777777777777777777777777777777777777377777777777777777777777777777777737777373777737577777300424000400000000000000000000000000000000777174777756765404051425373735737777777777777777777777777777777777777777777777777777777777777377777577777777777777777375777737777353777100100400040400000000000000000000000000007717137577764767404061777777777777737737777777777777777377777777737537777777777777777577777777773773777737775377177577737353753737770737100400400000000000000000000000000000000077717177777467760030065377577777777777777777777777377777777777777777777777777777777373735371777775777177753777777737717757775375753573536100050040404040000000000000000000000000771717177720767000043737737737737757737773773777777777777777777777777777777777777577777777777737773777777777777777777773773737737377357753000004200000004040000000000000000000047773537777504004104375777573757777371777777777773777777777777777777777777777777373777777777777777777777777777777777777777757777777377373777200504040404000000400000000000000000077153577770000016075375373777737177777717717777777777777777777777777777777777777777777777777777777777777777777777777777777375373577177573535300100040104004000040400040040004000177353577770070007277377777537777753757777777777777777777777777777777777777777777777777777777757777777773777577777775377537727576377717252734120050040400404040000040000000400007735353777005006535357777737771773777377777777777777777777777777777777777777777777775737777377777717377777777773777777777753753735752771775173500007000040000004004000400400000477717177775004353777737377773777777777777777777777777777777777777777777777777777773737757377173717777773577737777773777773773777773771773136343700000561040405004000400400040400775317777700367771737577537757777777777777777777777777377777777777777777777777775757717777777777737177577377777775777773777353717773771776535353716000047000404004000500050010001735717777761717777573777777777777777777777777777757375777777777777777777777773737737773753777177577737777537537737777757777777771757372537737271717100005252004004040604004040077531717777177777777777777737777777775777777777777777777777777777777777777777757717753757775377737737773777777777777777777177173777737753770775363774320000416524100000400400004773717777777777737777777777777777377377777777777777777777777777777777777777777737773777773777777777577757377377777777377377777753737753771775375757377577600000106141410143405007757537777777777777377737777773777777777777777777777777777777777777777777777777753777737777777777777737777777777777777777777377777573777777377373775373735373000000000400010000077377717777737777757757571777777777777777777777777777777777777777777777777737773777777777777777777777777777777777777777777777777777777737775777777377775777777777161612161637777777101777777771771773777777777777777777777777777777777777777777177577377577757777777777777777777777777777777777737777777777777737737775773737717717771737737537777777777777777775717177777771777777777377773777777777777777777777777777777777777777777777777777777777777777777377377777377777777777377577177537777777373757737737735377735737737377737775773777377717177777777737777777777777777777377777777777777777777777777777777777777777777777357537537777577773775753573577577537377737753757357757357571753777171735735775357537737571777771717577777777777375777375735377377775377777777777777777777777777777777777777777777737777771773753757377377777737777777777773777377737737737377375377777737573537737753773777777777177777777775775737757737777777757377777777777777777777777777777777777777777357777777777777777777777777777777773777777777777777777777777777777777537717773777777777777577777717711737777173737377777377777777177377777777777777777777777777777777777777777777737377777777777777777777777773777777777737775777777777757777775373737777773777377377537737777777710101417777757757377777771735377777777777777777777777777777777777777777777777777777777377777777377377777777777775775775775737777717717371735377575735373757175365737777773737777777773617377373775737773777777777777777777777777777777777777777777777777777777377757177573737777577773575373573737737777773773737777777777777737373777175337637173573537777577717777753775777775377777777777777777777777777777777777777777777777777777777777777773737773777573573753777737777777777773773777577577737353717353577175217437753577377377771737373773777375377375377777777777777777777777777777777777777777777777777777777777777757153471773737373773771737771737377777777777773777737577777777777377737733717373717177737777777577777375377777777777777777777777777777777777777777777777777777777777777777777773737773771757577573577377717777575717377777777777377773717353717357175717577717753777175773773537777777777777777777777777777777777777777777777777777777777777777777777777777777753473535377373717353717171735373737777777777777777737777777777777737737737353735371737737777377777777777777777777777777777777777777777777777777777777777777777777777777777777777773777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777777773535000000000000000000000105000000000000C7AD05FE)",
                assert);

            AssertFileContainsString(
                filePath,
                "INSERT [dbo].[Order Details] ([OrderID], [ProductID], [UnitPrice], [Quantity], [Discount]) VALUES (10248, 11, 14.0000, 12, 0)",
                assert);
        }

        private static void AssertFileContainsString(string filePath, string str, bool assertTrue)
        {
            string fileText = File.ReadAllText(filePath);
            bool found = fileText.Contains(str);
            if (assertTrue)
            {
                Assert.True(found, string.Format("The string '{0}' was not found in file.", str));
            }
            else
            {
                Assert.False(found, string.Format("The string '{0}' was found in file.", str));
            }
        }


        public class ScriptingFixture : IDisposable
        {
            public ScriptingFixture()
            {
                this.Database = SqlTestDb.CreateNew(TestServerType.OnPrem);
                this.Database.RunQuery(Scripts.CreateNorthwindSchema, throwOnError: true);
                Console.WriteLine("Northwind setup complete, database name: {0}", this.Database.DatabaseName);
            }

            /// <summary>
            /// The count of object when scripting the entire database including the database object.
            /// </summary>
            public const int ObjectCountWithDatabase = 46;

            /// <summary>
            /// The count of objects when scripting the entire database excluding the database object.
            /// </summary>
            public const int ObjectCountWithoutDatabase = 45;

            public SqlTestDb Database { get; private set; }

            public void Dispose()
            {
                if (this.Database != null)
                {
                    Console.WriteLine("Northwind cleanup, deleting database name: {0}", this.Database.DatabaseName);
                    this.Database.Dispose();
                }
            }
        }
   }
}
