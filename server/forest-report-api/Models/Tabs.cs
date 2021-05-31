using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.Internal;
using forest_report_api.Extensions;
using forest_report_api.Helper;

namespace forest_report_api.Models
{
    public class Tabs
    {
        public Tabs()
        {
        }

        public Tabs(int? quarter, int? year)
        {
            if (quarter != null && quarter.Value != 0 && year != null)
            {
                Models = null;
                Models = new List<TabModel>()
                {
                    _balanceTabModel.DeepClone(), _profitLossTabModel.DeepClone(),
                    _changeEquityTabModel.DeepClone(), _moveMoneyTabModel.DeepClone(), _decodingAccruedTaxes.DeepClone(),
                    _decodingFixedAssets.DeepClone()
                };
                TabModel balance;
                balance = Models.FirstOrDefault(x => x.TabName == TabName.BalanceTab);
                switch (quarter)
                {
                    case 1:
                        SetTitles(balance, $"За январь-март {year} года",
                            $"на {new DateTime(year.Value, 3, DateTime.DaysInMonth(year.Value, 3)):dd.MM.yyyy} года",
                            new Dictionary<string, string>
                            {
                                { "value1", $"На {DateTime.DaysInMonth(year.Value,3)} марта {year.Value} года"},
                                { "value2", $"На {DateTime.DaysInMonth(year.Value - 1 ,12)} декабря {year.Value - 1 } года"}
                            },
                            new Dictionary<string, string>
                            {
                                {"{период}", $"январь-март {year - 1} года"},
                                {"{период2}", $"январь-март {year} года"},
                                {"{дату2}", new DateTime(year.Value - 1, 3, DateTime.DaysInMonth(year.Value - 1, 3)).ToString("dd.MM.yyyy")},
                                {"{дату3}", new DateTime(year.Value, 3, DateTime.DaysInMonth(year.Value, 3)).ToString("dd.MM.yyyy")}
                            },
                            year.Value);
                        break;
                    case 2:
                        SetTitles(balance, $"За январь-июнь {year} года",
                            $"на {new DateTime(year.Value, 6, DateTime.DaysInMonth(year.Value, 6)):dd.MM.yyyy} года",
                            new Dictionary<string, string>
                            {
                                { "value1", $"На {DateTime.DaysInMonth(year.Value,6)} июня {year.Value} года"},
                                { "value2", $"На {DateTime.DaysInMonth(year.Value - 1 ,12)} декабря {year.Value - 1 } года"}
                            },
                            new Dictionary<string, string>
                            {
                                {"{период}", $"январь-июнь {year - 1} года"},
                                {"{период2}", $"январь-июнь {year} года"},
                                {"{дату2}", new DateTime(year.Value - 1, 6, DateTime.DaysInMonth(year.Value - 1, 6)).ToString("dd.MM.yyyy")},
                                {"{дату3}", new DateTime(year.Value, 6, DateTime.DaysInMonth(year.Value, 6)).ToString("dd.MM.yyyy")}
                            },
                            year.Value);
                        break;
                    case 3:
                        SetTitles(balance, $"За январь-сентябрь {year} года",
                            $"на {new DateTime(year.Value, 9, DateTime.DaysInMonth(year.Value, 9)):dd.MM.yyyy} года",
                            new Dictionary<string, string>
                            {
                                { "value1", $"На {DateTime.DaysInMonth(year.Value,9)} сентября {year.Value} года"},
                                { "value2", $"На {DateTime.DaysInMonth(year.Value - 1 ,12)} декабря {year.Value - 1 } года"}
                            },
                            new Dictionary<string, string>
                            {
                                {"{период}", $"январь-сентябрь {year - 1} года"},
                                {"{период2}", $"январь-сентябрь {year} года"},
                                {"{дату2}", new DateTime(year.Value - 1, 9, DateTime.DaysInMonth(year.Value - 1, 9)).ToString("dd.MM.yyyy")},
                                {"{дату3}", new DateTime(year.Value, 9, DateTime.DaysInMonth(year.Value, 9)).ToString("dd.MM.yyyy")}
                            },
                            year.Value);
                        break;
                    case 4:
                        SetTitles(balance, $"За январь-декабрь {year} года",
                            $"на {new DateTime(year.Value, 12, DateTime.DaysInMonth(year.Value, 12)):dd.MM.yyyy} года",
                            new Dictionary<string, string>
                            {
                                { "value1", $"На {DateTime.DaysInMonth(year.Value,12)} декабря {year.Value} года"},
                                { "value2", $"На {DateTime.DaysInMonth(year.Value - 1 ,12)} декабря {year.Value - 1 } года"}
                            },
                            new Dictionary<string, string>
                            {
                                {"{период}", $"январь-декабрь {year - 1} года"},
                                {"{период2}", $"январь-декабрь {year} года"},
                                {"{дату2}", new DateTime(year.Value - 1, 12, DateTime.DaysInMonth(year.Value - 1, 12)).ToString("dd.MM.yyyy")},
                                {"{дату3}", new DateTime(year.Value, 12, DateTime.DaysInMonth(year.Value, 12)).ToString("dd.MM.yyyy")}
                            },
                            year.Value);
                        break;
                }
            }
        }

        private void SetTitles(TabModel balance, string another, string balanceTab, Dictionary<string,string> balanceTabTitle, Dictionary<string,string> changeEquityTab, int year)
        {
            Models.Where(x => x.TabName != TabName.BalanceTab)
                .ForAll(model => model.TitleDate = another);
            if (balance != null)
            {
                balance.TitleDate = balanceTabTitle.ContainsKey("value1") ? balanceTabTitle["value1"] : string.Empty;
                balance.Table.Columns.FirstOrDefault(x => x.DataIndex == "value1")!.Title =
                    balanceTabTitle.ContainsKey("value1") ? balanceTabTitle["value1"] : string.Empty;
                balance.Table.Columns.FirstOrDefault(x => x.DataIndex == "value2")!.Title =
                    balanceTabTitle.ContainsKey("value2") ? balanceTabTitle["value2"] : string.Empty;
            }

            var moveTab = Models.FirstOrDefault(x => x.TabName == TabName.MoveMoneyTab);
            if (moveTab != null)
            {
                moveTab.Table.Columns.FirstOrDefault(x => x.DataIndex == "value1")!.Title = moveTab.TitleDate;
                moveTab.Table.Columns.FirstOrDefault(x => x.DataIndex == "value2")!.Title =
                    moveTab.TitleDate.Replace($"{year}", $"{year - 1}");

                var changeMoveItems = moveTab.Table.Rows.Select(x => (MoveMoneyItem) x).ToArray();
                
                var codeForOneYearAgo = new[] {"120"};
                foreach (var changeMoveItem in changeMoveItems
                    .Where(x => codeForOneYearAgo.Contains(x.CodeItem)))
                    changeMoveItem.Title = changeMoveItem.Title.Replace("{дату3}", $"31.12.{year - 1}");

                var dictionaryValue = changeEquityTab.ContainsKey("{дату3}") ? changeEquityTab["{дату3}"] : null;
                if (dictionaryValue != null)
                {
                    var codesEndPeriods = new[] {"130"};
                    foreach (var changeMoveItem in changeMoveItems.Where(x => codesEndPeriods.Contains(x.CodeItem)))
                        changeMoveItem.Title = changeMoveItem.Title.Replace("{дату4}", dictionaryValue);
                }
            }

            var equityTab = Models.FirstOrDefault(x => x.TabName == TabName.ChangeEquityTab);
            if (equityTab != null)
            {
                var changeEquityItems = equityTab.Table.Rows.Select(x => (ChangeEquityItem) x).ToArray();

                var codeForTwoYearAgo = new[] {"010", "040", "131"};
                foreach (var changeEquityItem in changeEquityItems
                    .Where(x => codeForTwoYearAgo.Contains(x.CodeItem)))
                    changeEquityItem.Title = changeEquityItem.Title.Replace("{дату}", $"31.12.{year - 2}");
                
                var codeForOneYearAgo = new[] {"110","140"};
                foreach (var changeEquityItem in changeEquityItems
                    .Where(x => codeForOneYearAgo.Contains(x.CodeItem)))
                    changeEquityItem.Title = changeEquityItem.Title.Replace("{дату2}", $"31.12.{year - 1}");

                var dictionaryValue = changeEquityTab.ContainsKey("{период}") ? changeEquityTab["{период}"] : null;
                if (dictionaryValue != null)
                {
                    var codesPeriods = new[] {"050", "060"};
                    foreach (var changeEquityItem in changeEquityItems.Where(x => codesPeriods.Contains(x.CodeItem)))
                        changeEquityItem.Title = changeEquityItem.Title.Replace("{период}", dictionaryValue);
                }

                dictionaryValue = changeEquityTab.ContainsKey("{дату2}") ? changeEquityTab["{дату2}"] : null;
                if (dictionaryValue != null)
                {
                    var codesEndPeriods = new[] {"100"};
                    foreach (var changeEquityItem in changeEquityItems.Where(x => codesEndPeriods.Contains(x.CodeItem)))
                        changeEquityItem.Title = changeEquityItem.Title.Replace("{дату2}", dictionaryValue);
                }

                dictionaryValue = changeEquityTab.ContainsKey("{период2}") ? changeEquityTab["{период2}"] : null;
                if (dictionaryValue != null)
                {
                    var codesEndPeriods = new[] {"150", "160"};
                    foreach (var changeEquityItem in changeEquityItems.Where(x => codesEndPeriods.Contains(x.CodeItem)))
                        changeEquityItem.Title = changeEquityItem.Title.Replace("{период2}", dictionaryValue);
                }

                dictionaryValue = changeEquityTab.ContainsKey("{дату3}") ? changeEquityTab["{дату3}"] : null;
                if (dictionaryValue != null)
                {
                    var codesEndPeriods = new[] {"200"};
                    foreach (var changeEquityItem in changeEquityItems.Where(x => codesEndPeriods.Contains(x.CodeItem)))
                        changeEquityItem.Title = changeEquityItem.Title.Replace("{дату3}", dictionaryValue);
                }
            }
        }

        public IEnumerable<TabModel> Models = new List<TabModel>
        {
            _balanceTabModel, _profitLossTabModel,
            _changeEquityTabModel, _moveMoneyTabModel, _decodingAccruedTaxes,
            _decodingFixedAssets
        };

        private static TabModel _balanceTabModel = new()
        {
            TabId = 1,
            Title = "Бухгалтерский баланс",
            Attachment = new
            {
                Title = "Приложение 1", Text =
                    "к Национальному стандарту бухгалтерского учета и <br>отчетности \"Индивидуальная бухгалтерская отчетность\",<br>" +
                    "утвержденному постановлением Минфина Республики Беларусь <br>от 12.12.2016 №104"
            },
            TabName = TabName.BalanceTab,
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            SubtractedRows = new[] {"112", "420", "430"},
            Bindings = new List<Binding>
            {
                new()
                {
                    Target = "110",
                    Operations = "-",
                    From = "111,112",
                    Type = "row"
                },
                new()
                {
                    Target = "130",
                    Operations = "+,+",
                    From = "131,132,133",
                    Type = "row"
                },
                new()
                {
                    Target = "190",
                    Operations = "+,+,+,+,+,+,+",
                    From = "110,120,130,140,150,160,170,180",
                    Type = "row"
                },
                new()
                {
                    Target = "210",
                    Operations = "+,+,+,+,+",
                    From = "211,212,213,214,215,216",
                    Type = "row"
                },
                new()
                {
                    Target = "290",
                    Operations = "+,+,+,+,+,+,+",
                    From = "210,220,230,240,250,260,270,280",
                    Type = "row"
                },
                new()
                {
                    Target = "300",
                    Operations = "+",
                    From = "190,290",
                    Type = "row"
                },
                new()
                {
                    Target = "490",
                    Operations = "-,-,+,+,+,+,+",
                    From = "410,420,430,440,450,460,470,480",
                    Type = "row"
                },
                new()
                {
                    Target = "590",
                    Operations = "+,+,+,+,+",
                    From = "510,520,530,540,550,560",
                    Type = "row"
                },
                new()
                {
                    Target = "630",
                    Operations = "+,+,+,+,+,+,+",
                    From = "631,632,633,634,635,636,637,638",
                    Type = "row"
                },
                new()
                {
                    Target = "690",
                    Operations = "+,+,+,+,+,+",
                    From = "610,620,630,640,650,660,670",
                    Type = "row"
                },
                new()
                {
                    Target = "700",
                    Operations = "+,+",
                    From = "490,590,690",
                    Type = "row"
                }
            },
            AdditionTable = new AdditionTable(),
            Table = new Table
            {
                Columns = new List<ColumnItem>()
                {
                    new() {Title = "Активы", DataIndex = "title"},
                    new() {Title = "Код строки", DataIndex = "codeItem"},
                    new() {Title = "На дату", DataIndex = "value1"},
                    new() {Title = "На дату", DataIndex = "value2"}
                },
                Rows = new List<BalanceSheetItem>
                {
                    new()
                    {
                        TitlePosition = 1,
                        Title = "I Долгосрочные активы"
                    },
                    new()
                    {
                        Title = "Основные средства",
                        CodeItem = "110"
                    },
                    new()
                    {
                        Title = "Первоначальная стоимость",
                        CodeItem = "111",
                        CodeParent = "110"
                    },
                    new()
                    {
                        Title = "Амортизация",
                        CodeItem = "112",
                        CodeParent = "110"
                    },
                    new()
                    {
                        Title = "Нематериальные активы",
                        CodeItem = "120"
                    },
                    new()
                    {
                        Title = "Доходные вложения в материальные активы",
                        CodeItem = "130"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title = "инвестиционная недвижимость",
                        CodeItem = "131",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "предметы финансовой аренды (лизинга)",
                        CodeItem = "132",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "прочие доходные вложения в материальные активы",
                        CodeItem = "133",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "Вложения в долгосрочные активы",
                        CodeItem = "140"
                    },
                    new()
                    {
                        Title = "Долгосрочные финансовые вложения",
                        CodeItem = "150"
                    },
                    new()
                    {
                        Title = "Отложенные налоговые активы",
                        CodeItem = "160"
                    },
                    new()
                    {
                        Title = "Долгосрочная дебиторская задолженность",
                        CodeItem = "170"
                    },
                    new()
                    {
                        Title = "в т.ч. Сумма созданного резерва по сомнительным долгам",
                        CodeItem = "171",
                        CodeParent = "170",
                    },
                    new()
                    {
                        Title = "Прочие долгосрочные активы",
                        CodeItem = "180"
                    },
                    new()
                    {
                        Title = "ИТОГО по разделу I",
                        CodeItem = "190"
                    },
                    new()
                    {
                        Title = "II КРАТКОСРОЧНЫЕ АКТИВЫ",
                        TitlePosition = 2
                    },
                    new()
                    {
                        Title = "Запасы",
                        CodeItem = "210"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title = "материалы",
                        CodeItem = "211",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "животные на выращивании и откорме",
                        CodeItem = "212",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "незавершенное производство",
                        CodeItem = "213",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "готовая продукция и товары",
                        CodeItem = "214",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "товары отгруженные",
                        CodeItem = "215",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "прочие запасы",
                        CodeItem = "216",
                        CodeParent = "210"
                    },
                    new()
                    {
                        Title = "Долгосрочные активы, предназначенные для реализации",
                        CodeItem = "220"
                    },
                    new()
                    {
                        Title = "Расходы будущих периодов",
                        CodeItem = "230"
                    },
                    new()
                    {
                        Title = "НДС по приобретенным товарам, работам, услугам",
                        CodeItem = "240",
                    },
                    new()
                    {
                        Title = "в т.ч. НДС по приобретенным ценностям, входящим в состав оборотных активов",
                        CodeItem = "241",
                        CodeParent = "240"
                    },
                    new()
                    {
                        Title = "Краткосрочная дебиторская задолженность",
                        CodeItem = "250"
                    },
                    new()
                    {
                        Title = "из стр. 250 по расчетам с бюджетов",
                        CodeItem = "251",
                        CodeParent = "250"
                    },
                    new()
                    {
                        Title = "из стр. 250 по социальному страхованию и обеспечению",
                        CodeItem = "252",
                        CodeParent = "250"
                    },
                    new()
                    {
                        Title = "в т.ч. Сумма созданного резерва по сомнительным долгам",
                        CodeItem = "253",
                        CodeParent = "250"
                    },
                    new()
                    {
                        Title = "Краткосрочные финансовые вложения",
                        CodeItem = "260"
                    },
                    new()
                    {
                        Title = "Денежные средства и эквиваленты",
                        CodeItem = "270"
                    },
                    new()
                    {
                        Title = "Прочие краткосрочные активы",
                        CodeItem = "280"
                    },
                    new()
                    {
                        Title = "ИТОГО по разделу II",
                        CodeItem = "290"
                    },
                    new()
                    {
                        Title = "БАЛАНС",
                        CodeItem = "300"
                    },
                    new()
                    {
                        Title = "Справочно: стоимость имущества, переданного по договору безвозмездного пользования",
                        CodeItem = "301"
                    },
                    new()
                    {
                        Title = "III. СОБСТВЕННЫЙ КАПИТАЛ",
                        TitlePosition = 3
                    },
                    new()
                    {
                        Title = "Уставный капитал",
                        CodeItem = "410"
                    },
                    new()
                    {
                        Title = "Неоплаченная часть уставного капитала",
                        CodeItem = "420"
                    },
                    new()
                    {
                        Title = "Собственные акции (доли в уставном капитале)",
                        CodeItem = "430"
                    },
                    new()
                    {
                        Title = "Резервный капитал",
                        CodeItem = "440"
                    },
                    new()
                    {
                        Title = "Добавочный капитал",
                        CodeItem = "450"
                    },
                    new()
                    {
                        Title = "Нераспределенная прибыль (непокрытый убыток)",
                        CodeItem = "460"
                    },
                    new()
                    {
                        Title = "Чистая прибыль (убыток) отчетного периода",
                        CodeItem = "470"
                    },
                    new()
                    {
                        Title = "Целевое финансирование",
                        CodeItem = "480"
                    },
                    new()
                    {
                        Title = "ИТОГО по разделу III",
                        CodeItem = "490"
                    },
                    new()
                    {
                        Title = "IV. ДОЛГОСРОЧНЫЕ ОБЯЗАТЕЛЬСТВА",
                        TitlePosition = 4
                    },
                    new()
                    {
                        Title = "Долгосрочные кредиты и займы",
                        CodeItem = "510"
                    },
                    new()
                    {
                        Title = "Долгосрочные обязательства по лизинговым платежам",
                        CodeItem = "520"
                    },
                    new()
                    {
                        Title = "Отложенные налоговые обязательства",
                        CodeItem = "530"
                    },
                    new()
                    {
                        Title = "Доходы будущих периодов",
                        CodeItem = "540"
                    },
                    new()
                    {
                        Title = "Резервы предстоящих платежей",
                        CodeItem = "550"
                    },
                    new()
                    {
                        Title = "Прочие долгосрочные обязательства",
                        CodeItem = "560"
                    },
                    new()
                    {
                        Title = "ИТОГО по разделу IV",
                        CodeItem = "590"
                    },
                    new()
                    {
                        Title = "V. КРАТКОСРОЧНЫЕ ОБЯЗАТЕЛЬСТВА",
                        TitlePosition = 5
                    },
                    new()
                    {
                        Title = "Краткосрочные кредиты и займы",
                        CodeItem = "610"
                    },
                    new()
                    {
                        Title = "Краткосрочная часть долгосрочные обязательств",
                        CodeItem = "620"
                    },
                    new()
                    {
                        Title = "Краткосрочная кредиторская задолженность",
                        CodeItem = "630"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title = "поставщикам, подрядчикам, исполнителям",
                        CodeItem = "631",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "по авансам полученным",
                        CodeItem = "632",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "по налогам и сборам",
                        CodeItem = "633",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "по социальному страхованию и обеспечению",
                        CodeItem = "634",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "по оплате труда",
                        CodeItem = "635",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "по лизинговым платежам",
                        CodeItem = "636",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "собственнику имущества (учредителямб участникам)",
                        CodeItem = "637",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "прочим кредиторам",
                        CodeItem = "638",
                        CodeParent = "630"
                    },
                    new()
                    {
                        Title = "Обязательства, предназначенные для реализации",
                        CodeItem = "640"
                    },
                    new()
                    {
                        Title = "Доходы будущих периодов",
                        CodeItem = "650"
                    },
                    new()
                    {
                        Title = "Резервы предстоящих платежей",
                        CodeItem = "660"
                    },
                    new()
                    {
                        Title = "Прочие краткосрочные обязательства",
                        CodeItem = "670"
                    },
                    new()
                    {
                        Title = "ИТОГО по разделу V",
                        CodeItem = "690"
                    },
                    new()
                    {
                        Title = "БАЛАНС",
                        CodeItem = "700"
                    }
                },
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };

        private static TabModel _profitLossTabModel = new()
        {
            TabId = 2,
            TabName = TabName.ProfitLossTab,
            Title = "Отчет о прибылях и убытках",
            Attachment = new
            {
                Title = "Приложение 2", Text =
                    "к Национальному стандарту бухгалтерского учета и <br>отчетности \"Индивидуальная бухгалтерская отчетность\",<br>" +
                    "утвержденному постановлением Минфина Республики Беларусь <br>от 12.12.2016 №104"
            },
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            SubtractedRows = new[] {"020", "040", "050", "080","110","111","112", "130", "131","132","133", "160", "190", "200"},
            Bindings = new List<Binding>
            {
                new()
                {
                    Target = "030",
                    Operations = "-",
                    From = "010,020",
                    Type = "row"
                },
                new()
                {
                    Target = "060",
                    Operations = "-,-",
                    From = "030,040,050",
                    Type = "row"
                },
                new()
                {
                    Target = "090",
                    Operations = "+,-",
                    From = "060,070,080",
                    Type = "row"
                },
                new()
                {
                    Target = "100",
                    Operations = "+,+,+",
                    From = "101,102,103,104",
                    Type = "row"
                },
                new()
                {
                    Target = "110",
                    Operations = "+",
                    From = "111,112",
                    Type = "row"
                },
                new()
                {
                    Target = "120",
                    Operations = "+",
                    From = "121,122",
                    Type = "row"
                },
                new()
                {
                    Target = "130",
                    Operations = "+,+",
                    From = "131,132,133",
                    Type = "row"
                },
                new()
                {
                    Target = "140",
                    Operations = "-,+,-",
                    From = "100,110,120,130",
                    Type = "row"
                },
                new()
                {
                    Target = "150",
                    Operations = "+",
                    From = "090,140",
                    Type = "row"
                },
                new()
                {
                    Target = "210",
                    Operations = "-,+,+,-,-",
                    From = "150,160,170,180,190,200",
                    Type = "row"
                },
                new()
                {
                    Target = "240",
                    Operations = "+,+",
                    From = "210,220,230",
                    Type = "row"
                }
            },
            Table = new Table
            {
                Columns = new List<ColumnItem>
                {
                    new() {Title = "Наименование показателей", DataIndex = "title"},
                    new() {Title = "Коды строк", DataIndex = "codeItem"},
                    new() {Title = "За отчетный период", DataIndex = "value1"},
                    new() {Title = "За аналогичный период прошлого года", DataIndex = "value2"}
                },
                Rows = new List<ProfitLossItem>
                {
                    new()
                    {
                        Title = "Выручка от реализации продукции, товаров, работ, услуг",
                        CodeItem = "010"
                    },
                    new()
                    {
                        Title = "Себестоимость реализованной продукции, товаров, работ, услуг",
                        CodeItem = "020"
                    },
                    new()
                    {
                        Title = "Валовая прибыль",
                        CodeItem = "030"
                    },
                    new()
                    {
                        Title = "Управленческие расходы",
                        CodeItem = "040"
                    },
                    new()
                    {
                        Title = "Расходы на реализацию",
                        CodeItem = "050"
                    },
                    new()
                    {
                        Title = "Прибыль (убыток) от реализации продукции, товаров, работ, услуг",
                        CodeItem = "060"
                    },
                    new()
                    {
                        Title = "Прочие доходы по текущей деятельности",
                        CodeItem = "070"
                    },
                    new()
                    {
                        Title = "Прочие расходы по текущей деятельности",
                        CodeItem = "080"
                    },
                    new()
                    {
                        Title = "Прибыль (убыток) от текущей деятельности",
                        CodeItem = "090"
                    },
                    new()
                    {
                        Title = "Доходы по инвестиционной деятельности",
                        CodeItem = "100"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title =
                            "доходы от выбытия основных средств, нематериальных активов и других долгосрочных активов",
                        CodeItem = "101",
                        CodeParent = "100"
                    },
                    new()
                    {
                        Title = "доходы от участия в уставном капитале других организаци",
                        CodeItem = "102",
                        CodeParent = "100"
                    },
                    new()
                    {
                        Title = "проценты к получению",
                        CodeItem = "103",
                        CodeParent = "100"
                    },
                    new()
                    {
                        Title = "прочие доходы по инвестиционной деятельности",
                        CodeItem = "104",
                        CodeParent = "100"
                    },
                    new()
                    {
                        Title = "Расходы по инвестиционной деятельности",
                        CodeItem = "110"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title =
                            "расходы от выбытия основных средств, нематериальных активов и других долгосрочных активов",
                        CodeItem = "111",
                        CodeParent = "110"
                    },
                    new()
                    {
                        Title = "прочие расходы по инвестиционной деятельности",
                        CodeItem = "112",
                        CodeParent = "110"
                    },
                    new()
                    {
                        Title = "Доходы по финансовой деятельности",
                        CodeItem = "120"
                    },
                    new()
                    {
                        Title = "в том числе:",
                    },
                    new()
                    {
                        Title = "курсовые разницы от пересчета активов и обязательств",
                        CodeItem = "121",
                        CodeParent = "120"
                    },
                    new()
                    {
                        Title = "прочие доходы по финансовой деятельности",
                        CodeItem = "122"
                    },
                    new()
                    {
                        Title = "Расходы по финансовой деятельности",
                        CodeItem = "130"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title = "проценты к уплате",
                        CodeItem = "131",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "курсовые разницы от пересчета активов и обязательств",
                        CodeItem = "132",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "прочие расходы по финансовой деятельности",
                        CodeItem = "133",
                        CodeParent = "130"
                    },
                    new()
                    {
                        Title = "Прибыль (убыток) от инвестиционной и финансовой деятельности",
                        CodeItem = "140"
                    },
                    new()
                    {
                        Title = "Прибыль (убыток) до налогообложения",
                        CodeItem = "150"
                    },
                    new()
                    {
                        Title = "Налог на прибыль",
                        CodeItem = "160"
                    },
                    new()
                    {
                        Title = "Изменение отложенных налоговых активов",
                        CodeItem = "170"
                    },
                    new()
                    {
                        Title = "Изменение отложенных налоговых обязательств",
                        CodeItem = "180"
                    },
                    new()
                    {
                        Title = "Прочие налоги и сборы, исчисляемые из прибыли (дохода)",
                        CodeItem = "190"
                    },
                    new()
                    {
                        Title = "Прочие платежи, исчисляемые из прибыли (дохода)",
                        CodeItem = "200"
                    },
                    new()
                    {
                        Title = "Чистая прибыль (убыток)",
                        CodeItem = "210"
                    },
                    new()
                    {
                        Title = "Результат от переоценки долгосрочных активов, не включаемый в чистую прибыль (убыток)",
                        CodeItem = "220"
                    },
                    new()
                    {
                        Title = "Результат от прочих операций, не включаемый в чистую прибыль (убыток)",
                        CodeItem = "230"
                    },
                    new()
                    {
                        Title = "Совокупная прибыль (убыток)",
                        CodeItem = "240"
                    },
                    new()
                    {
                        Title = "Базовая прибыль (убыток) на акцию",
                        CodeItem = "250"
                    },
                    new()
                    {
                        Title = "Разводненная прибыль (убыток) на акцию",
                        CodeItem = "260"
                    }
                }
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };

        private static TabModel _changeEquityTabModel = new()
        {
            TabId = 3,
            TabName = TabName.ChangeEquityTab,
            Title = "Отчет об изменении собственного капитала",
            Attachment = new
            {
                Title = "Приложение 3", Text =
                    "к Национальному стандарту бухгалтерского учета и <br>отчетности \"Индивидуальная бухгалтерская отчетность\",<br>" +
                    "утвержденному постановлением Минфина Республики Беларусь <br>от 12.12.2016 №104"
            },
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            SubtractedRows = new[] {"160","161","162","163","164","165","166","167","168","169"},
            SubtractedColumns = new[] {"value3"},
            Bindings = new List<Binding>()
            {
                new()
                {
                    Target = "value8",
                    Operations = "+,+,+,+,+,+",
                    From = "value1,value2,value3,value4,value5,value6,value7",
                    Type = "column"
                },
                new()
                {
                    Target = "040",
                    Operations = "+,+",
                    From = "010,020,030",
                    Type = "row"
                },
                new()
                {
                    Target = "050",
                    Operations = "+,+,+,+,+,+,+,+",
                    From = "051,052,053,054,055,056,057,058,059",
                    Type = "row"
                },
                new()
                {
                    Target = "060",
                    Operations = "+,+,+,+,+,+,+,+",
                    From = "061,062,063,064,065,066,067,068,069",
                    Type = "row"
                },
                new()
                {
                    Target = "100",
                    Operations = "+,+,+,+,+",
                    From = "040,050,060,070,080,090",
                    Type = "row"
                },
                new()
                {
                    Target = "140",
                    Operations = "+,+,+",
                    From = "110,120,130,131",
                    Type = "row"
                },
                new()
                {
                    Target = "150",
                    Operations = "+,+,+,+,+,+,+,+",
                    From = "151,152,153,154,155,156,157,158,159",
                    Type = "row"
                },
                new()
                {
                    Target = "160",
                    Operations = "+,+,+,+,+,+,+,+",
                    From = "161,162,163,164,165,166,167,168,169",
                    Type = "row"
                },
                new()
                {
                    Target = "200",
                    Operations = "+,-,+,+,+",
                    From = "140,150,160,170,180,190",
                    Type = "row"
                }
            },
            Table = new Table
            {
                Columns = new List<ColumnItem>
                {
                    new() {Title = "Наименование показателей", DataIndex = "title"},
                    new() {Title = "Коды строк", DataIndex = "codeItem"},
                    new() {Title = "Уставный капитал", DataIndex = "value1"},
                    new() {Title = "Неоплаченная часть уставного капитала", DataIndex = "value2"},
                    new() {Title = "Собственные акции (доли в уставном капитале)", DataIndex = "value3"},
                    new() {Title = "Резервный капитал", DataIndex = "value4"},
                    new() {Title = "Добавочный капитал", DataIndex = "value5"},
                    new() {Title = "Нераспределенная прибыль (непокрытый убыток)", DataIndex = "value6"},
                    new() {Title = "Чистая прибыль (убыток)", DataIndex = "value7"},
                    new() {Title = "Итого", DataIndex = "value8"},
                },
                Rows = new List<ChangeEquityItem>
                {
                    new()
                    {
                        Title = "Остаток на {дату}",
                        CodeItem = "010"
                    },
                    new()
                    {
                        Title = "Корректировка в связи с изменением учетной политики",
                        CodeItem = "020"
                    },
                    new()
                    {
                        Title = "Корректировки в связи с исправлением ошибок",
                        CodeItem = "030"
                    },
                    new()
                    {
                        Title = "Скорретированный остаток на {дату}",
                        CodeItem = "040"
                    },
                    new()
                    {
                        Title = "За {период}\n" +
                                "Увеличение собственного капитала",
                        CodeItem = "050"
                    },
                    new()
                    {
                        Title = "в том числе:"
                    },
                    new()
                    {
                        Title = "чистая прибыль",
                        CodeItem = "051",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "переоценка долгосрочных активов",
                        CodeItem = "052",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "доходы от прочих операций, не включаемые в чистую прибыль (убыток)",
                        CodeItem = "053",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "выпуск дополнительных акций",
                        CodeItem = "054",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "увеличение номинальной стоимости акций",
                        CodeItem = "055",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "вклады собственника имущества (учредителей, участников)",
                        CodeItem = "056",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "реорганизация",
                        CodeItem = "057",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "058",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "059",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "За {период}\n" +
                                "Уменьшение собственного капитала - всего",
                        CodeItem = "060"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "убыток",
                        CodeItem = "061",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "переоценка долгосрочных активов",
                        CodeItem = "062",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "расходы от прочих операций, не включаемые в чистую прибыль (убыток)",
                        CodeItem = "063",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "уменьшение номинальной стоимости акций",
                        CodeItem = "064",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "выкуп акций (долей в уставном капитале)",
                        CodeItem = "065",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "дивиденды и другие доходы от участия в уставном капитале организации",
                        CodeItem = "066",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "реорганизация",
                        CodeItem = "067",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "068",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "069",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "Изменение уставного капитала",
                        CodeItem = "070"
                    },
                    new()
                    {
                        Title = "Изменение резервного капитала",
                        CodeItem = "080"
                    },
                    new()
                    {
                        Title = "Изменение добавочного капитала",
                        CodeItem = "090"
                    },
                    new()
                    {
                        Title = "Остаток на {дату2}",
                        CodeItem = "100"
                    },
                    new()
                    {
                        Title = "Остаток на {дату2}",
                        CodeItem = "110"
                    },
                    new()
                    {
                        Title = "Корретировки в связи с изменением учетной политики",
                        CodeItem = "120"
                    },
                    new()
                    {
                        Title = "Корректировки в связи с исправлением ошибок",
                        CodeItem = "130"
                    },
                    new()
                    {
                        Title =
                            "Корректировки на сумму разниц от пересчета активов и обязательств в эквиваленте на {дату}",
                        CodeItem = "131"
                    },
                    new()
                    {
                        Title = "Скорректированный остаток на {дату2}",
                        CodeItem = "140"
                    },
                    new()
                    {
                        Title = "За {период2}\n" +
                                "Увеличение собственного капитала",
                        CodeItem = "150"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "чистая прибыль",
                        CodeItem = "151",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "переоценка долгосрочных активов",
                        CodeItem = "152",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "доходы от прочих операций, не включаемые в чистую прибыль (убыток)",
                        CodeItem = "153",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "выпуск дополнительных акций",
                        CodeItem = "154",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "увеличение номинальной стоимости акций",
                        CodeItem = "155",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "вклады собственника имущества (учредителей, участников)",
                        CodeItem = "156",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "реорганизация",
                        CodeItem = "157",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "158",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "159",
                        CodeParent = "150"
                    },
                    new()
                    {
                        Title = "За {период2}\n" +
                                "Уменьшение собственного капитала - всего",
                        CodeItem = "160"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "убыток",
                        CodeItem = "161",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "переоценка долгосрочных активов",
                        CodeItem = "162",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "расходы от прочих операций, не включаемые в чистую прибыль (убыток)",
                        CodeItem = "163",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "уменьшение номинальной стоимости акций",
                        CodeItem = "164",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "выкуп акций (долей в уставном капитале)",
                        CodeItem = "165",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "дивиденды и другие доходы от участия в уставном капитале организации",
                        CodeItem = "166",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "реорганизация",
                        CodeItem = "167",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "168",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "",
                        CodeItem = "169",
                        CodeParent = "160"
                    },
                    new()
                    {
                        Title = "Изменение уставного капитала",
                        CodeItem = "170"
                    },
                    new()
                    {
                        Title = "Изменение резервного капитала",
                        CodeItem = "180"
                    },
                    new()
                    {
                        Title = "Изменение добавочного капитала",
                        CodeItem = "190"
                    },
                    new()
                    {
                        Title = "Остаток на {дату3}",
                        CodeItem = "200"
                    },
                    new()
                    {
                        Title = "Прибыль, использованная на капитальные вложения в текущем году",
                        CodeItem = "201"
                    }
                }
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };

        private static TabModel _moveMoneyTabModel = new()
        {
            TabId = 4,
            TabName = TabName.MoveMoneyTab,
            Title = "Отчет о движении денежных средств",
            Attachment = new
            {
                Title = "Приложение 4", Text =
                    "к Национальному стандарту бухгалтерского учета и <br>отчетности \"Индивидуальная бухгалтерская отчетность\",<br>" +
                    "утвержденному постановлением Минфина Республики Беларусь <br>от 12.12.2016 №104"
            },
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            SubtractedRows = new[] {"030","031","032","033","034", "060","061","062","063","064", "090", "091","092","093","094","095"},
            Bindings = new List<Binding>
            {
                new()
                {
                    Target = "020",
                    Operations = "+,+,+",
                    From = "021,022,023,024",
                    Type = "row"
                },
                new()
                {
                    Target = "030",
                    Operations = "+,+,+",
                    From = "031,032,033,034",
                    Type = "row"
                },
                new()
                {
                    Target = "040",
                    Operations = "-",
                    From = "020,030",
                    Type = "row"
                },
                new()
                {
                    Target = "050",
                    Operations = "+,+,+,+",
                    From = "051,052,053,054,055",
                    Type = "row"
                },
                new()
                {
                    Target = "060",
                    Operations = "+,+,+",
                    From = "061,062,063,064",
                    Type = "row"
                },
                new()
                {
                    Target = "070",
                    Operations = "-",
                    From = "050,060",
                    Type = "row"
                },
                new()
                {
                    Target = "080",
                    Operations = "+,+,+",
                    From = "081,082,083,084",
                    Type = "row"
                },
                new()
                {
                    Target = "090",
                    Operations = "+,+,+,+",
                    From = "091,092,093,094,095",
                    Type = "row"
                },
                new()
                {
                    Target = "100",
                    Operations = "-",
                    From = "080,090",
                    Type = "row"
                },
                new()
                {
                    Target = "110",
                    Operations = "+,+",
                    From = "040,070,100",
                    Type = "row"
                },
                new()
                {
                    Target = "130",
                    Operations = "+",
                    From = "110,120",
                    Type = "row"
                }
            },
            Table = new Table
            {
                Columns = new List<ColumnItem>
                {
                    new() {Title = "Наименование показателей", DataIndex = "title"},
                    new() {Title = "Коды строк", DataIndex = "codeItem"},
                    new() {Title = "За дату", DataIndex = "value1"},
                    new() {Title = "За дату", DataIndex = "value2"},
                },
                Rows = new List<MoveMoneyItem>
                {
                    new()
                    {
                        Title = "Движение денежных средств по текущей деятельности",
                        CodeItem = "020"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "от покупателей продукции, товаров, заказчиков работ, услуг",
                        CodeItem = "021",
                        CodeParent = "020"
                    },
                    new()
                    {
                        Title = "от покупателей материалов и других запасов",
                        CodeItem = "022",
                        CodeParent = "020"
                    },
                    new()
                    {
                        Title = "роялти",
                        CodeItem = "023",
                        CodeParent = "020"
                    },
                    new()
                    {
                        Title = "прочие поступления",
                        CodeItem = "024",
                        CodeParent = "020"
                    },
                    new()
                    {
                        Title = "Направлено денежных средств - всего",
                        CodeItem = "030"
                    },
                    new()
                    {
                        Title = "В том числе:",
                    },
                    new()
                    {
                        Title = "на приобретение запасов, работ, услуг",
                        CodeItem = "031",
                        CodeParent = "030"
                    },
                    new()
                    {
                        Title = "на оплату труда",
                        CodeItem = "032",
                        CodeParent = "030"
                    },
                    new()
                    {
                        Title = "на оплату налогов и сборов",
                        CodeItem = "033",
                        CodeParent = "030"
                    },
                    new()
                    {
                        Title = "на прочие выплаты",
                        CodeItem = "034",
                        CodeParent = "030"
                    },
                    new()
                    {
                        Title = "Результат движения денежных средств по текущей деятельности",
                        CodeItem = "040"
                    },
                    new()
                    {
                        Title = "Поступило денежных средств - всего",
                        CodeItem = "050"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title =
                            "от покупателей основных средств, нематериаольных активов и других долгосрочных активов",
                        CodeItem = "051",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "возврат предоставленных займов",
                        CodeItem = "052",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "доходы от участия в уставном капитале других организаций",
                        CodeItem = "053",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "проценты",
                        CodeItem = "054",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "прочие поступления",
                        CodeItem = "055",
                        CodeParent = "050"
                    },
                    new()
                    {
                        Title = "Направлено денежных средств - всего",
                        CodeItem = "060"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title =
                            "на приобретение и создание основных средств, нематериальных активов и других долгосрочных активов",
                        CodeItem = "061",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "на предоставление займов",
                        CodeItem = "062",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "на вклады в уставный капитал других организаций",
                        CodeItem = "063",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "прочие выплаты",
                        CodeItem = "064",
                        CodeParent = "060"
                    },
                    new()
                    {
                        Title = "Результат движения денежных средств по инвестиционной деятельности",
                        CodeItem = "070"
                    },
                    new()
                    {
                        Title = "Поступило денежных средств",
                        CodeItem = "080"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "кредиты и займы",
                        CodeItem = "081",
                        CodeParent = "080"
                    },
                    new()
                    {
                        Title = "от выпуска акций",
                        CodeItem = "082",
                        CodeParent = "080"
                    },
                    new()
                    {
                        Title = "вклады собственника имущества (учредителей, участников)",
                        CodeItem = "083",
                        CodeParent = "080"
                    },
                    new()
                    {
                        Title = "прочие поступления",
                        CodeItem = "084",
                        CodeParent = "080"
                    },
                    new()
                    {
                        Title = "Направлено денежных средств - всего",
                        CodeItem = "090"
                    },
                    new()
                    {
                        Title = "В том числе:"
                    },
                    new()
                    {
                        Title = "на погашение кредитов и займов",
                        CodeItem = "091",
                        CodeParent = "090"
                    },
                    new()
                    {
                        Title = "на выплаты дивидентов и других доходов от участия в уставном капитале организации",
                        CodeItem = "092",
                        CodeParent = "090"
                    },
                    new()
                    {
                        Title = "на выплаты процентов",
                        CodeItem = "093",
                        CodeParent = "090"
                    },
                    new()
                    {
                        Title = "на лизинговые платежи",
                        CodeItem = "094",
                        CodeParent = "090"
                    },
                    new()
                    {
                        Title = "другие выплаты",
                        CodeItem = "095",
                        CodeParent = "090"
                    },
                    new()
                    {
                        Title = "Результат движения денежных средств по финансовой деятельности",
                        CodeItem = "100"
                    },
                    new()
                    {
                        Title =
                            "Результат движения денежных средств по текущей, инвестиционной и финансовой деятельности",
                        CodeItem = "110"
                    },
                    new()
                    {
                        Title = "Остаток денежных средств и эквивалентов денежных средств на {дату3}",
                        CodeItem = "120"
                    },
                    new()
                    {
                        Title = "Остаток денежных средств и эквивалентов денежных средств на {дату4}",
                        CodeItem = "130"
                    },
                    new()
                    {
                        Title = "Влияние изменений курса иностранной валюты",
                        CodeItem = "140"
                    }
                }
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };

        private static TabModel _decodingAccruedTaxes = new()
        {
            TabId = 5,
            TabName = TabName.DecodingAccruedTaxes,
            Title = "Расшифровка по начисленным и уплаченным налогам и сборам",
            Attachment = new
            {
                Title = "Приложение 6", Text = ""
            },
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            Table = new Table
            {
                Columns = new List<ColumnItem>
                {
                    new() {Title = "Наименование показателей", DataIndex = "title"},
                    new() {Title = "Коды строк", DataIndex = "codeItem"},
                    new() {Title = "Причитается по расчету", DataIndex = "value1"},
                    new() {Title = "Израсходовано", DataIndex = "value2"},
                    new() {Title = "Перечислено в бюджет и фонды", DataIndex = "value3"}
                },
                Rows = new List<DecodingAccruedTaxesItem>
                {
                    new()
                    {
                        Title = "Налог на недвижимость",
                        CodeItem = "100"
                    },
                    new()
                    {
                        Title = "Налог на прибыль",
                        CodeItem = "110"
                    },
                    new()
                    {
                        Title = "Налог на доходы - всего",
                        CodeItem = "120"
                    },
                    new()
                    {
                        Title = "налог на доходы в виде дивидентов",
                        CodeItem = "130"
                    },
                    new()
                    {
                        Title = "налог на доходы от операций ценными бумагами",
                        CodeItem = "140"
                    },
                    new()
                    {
                        Title = "Налог на добавленную стоимость",
                        CodeItem = "150"
                    },
                    new()
                    {
                        Title = "Налог с продаж автомобильного топлива",
                        CodeItem = "160"
                    },
                    new()
                    {
                        Title = "Акцизы",
                        CodeItem = "190"
                    },
                    new()
                    {
                        Title = "Земельный налог",
                        CodeItem = "200"
                    },
                    new()
                    {
                        Title = "Налог на пользование природными ресурсами (эколог)",
                        CodeItem = "220"
                    },
                    new()
                    {
                        Title = "Подоходный налог с физический лиц",
                        CodeItem = "230"
                    },
                    new()
                    {
                        Title = "Местные налоги и сборы",
                        CodeItem = "240"
                    },
                    new()
                    {
                        Title = "Прочие налоги и сборы",
                        CodeItem = "250"
                    },
                    new()
                    {
                        Title = "Экономические санкции",
                        CodeItem = "260"
                    },
                    new()
                    {
                        Title = "Отчисления от прибыли РУП",
                        CodeItem = "270"
                    },
                    new()
                    {
                        Title = "Социальное страхование и обеспечение",
                        CodeItem = "271"
                    },
                    new()
                    {
                        Title = "Инновационный фонд",
                        CodeItem = "280"
                    },
                    new()
                    {
                        Title = "в т.ч. из стр.280 инновационный фонд конц. (0,25%) от с/с",
                        CodeItem = "281"
                    },
                    new()
                    {
                        Title = "в т.ч. из стр.280 прочие инновационные фонды",
                        CodeItem = "282"
                    },
                    new()
                    {
                        Title = "Справочно: инвестиционный фонд",
                        CodeItem = "290"
                    }
                }
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };

        private static TabModel _decodingFixedAssets = new()
        {
            TabId = 6,
            TabName = TabName.DecodingFixedAssets,
            Title = "Расшифровка по основным средствам",
            Attachment = new
            {
                Title = "Приложение 7", Text = ""
            },
            Header = new Header
            {
                Government = "Совет министров",
                Unit = "тыс.руб."
            },
            SubtractedColumns = new[] {"value3"},
            ReadOnlyCells = new List<ValidationItem>
            {
                new()
                {
                    CodeItem = "020",
                    Cell = "value2"
                },
                new()
                {
                    CodeItem = "020",
                    Cell = "value3"
                },
                new()
                {
                    CodeItem = "021",
                    Cell = "value2"
                },
                new()
                {
                    CodeItem = "021",
                    Cell = "value3"
                },
                new()
                {
                    CodeItem = "100",
                    Cell = "value2"
                },
                new()
                {
                    CodeItem = "100",
                    Cell = "value3"
                },
                new()
                {
                    CodeItem = "010",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "011",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "012",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "013",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "014",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "015",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "016",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "017",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "018",
                    Cell = "value4"
                },
                new()
                {
                    CodeItem = "019",
                    Cell = "value4"
                }
            },
            Bindings = new List<Binding>
            {
                new()
                {
                    Target = "value4",
                    Operations = "+,-",
                    From = "value1,value2,value3",
                    Ignore = "020,021,100",
                    Type = "column"
                },
                new()
                {
                    Target = "010",
                    Operations = "+,+,+,+,+,+,+,+",
                    From = "011,012,013,014,015,016,017,018,019",
                    Type = "row"
                }
            },
            Table = new Table
            {
                Columns = new List<ColumnItem>
                {
                    new() {Title = "Наименование показателей", DataIndex = "title"},
                    new() {Title = "Коды строк", DataIndex = "codeItem"},
                    new() {Title = "На начало года", DataIndex = "value1"},
                    new() {Title = "Поступило", DataIndex = "value2"},
                    new() {Title = "Выбыло", DataIndex = "value3"},
                    new() {Title = "На конец года", DataIndex = "value4"}
                },
                Rows = new List<DecodingFixedAssetsItem>
                {
                    new()
                    {
                        Title = "Основные средства - всего",
                        CodeItem = "010"
                    },
                    new()
                    {
                        Title = "здания и сооружения",
                        CodeItem = "011"
                    },
                    new()
                    {
                        Title = "передаточные устройства",
                        CodeItem = "012"
                    },
                    new()
                    {
                        Title = "машины и оборудование",
                        CodeItem = "013"
                    },
                    new()
                    {
                        Title = "транспортные средства",
                        CodeItem = "014"
                    },
                    new()
                    {
                        Title = "инструмент, инвентарь и принадлежности",
                        CodeItem = "015"
                    },
                    new()
                    {
                        Title = "рабочий скот и животные основного фонда",
                        CodeItem = "016"
                    },
                    new()
                    {
                        Title = "многолетние насаждения",
                        CodeItem = "017"
                    },
                    new()
                    {
                        Title = "капитальные затраты в улучшение земель",
                        CodeItem = "018"
                    },
                    new()
                    {
                        Title = "прочие основные средства",
                        CodeItem = "019"
                    },
                    new()
                    {
                        Title = "Амортизация основных средств",
                        CodeItem = "020"
                    },
                    new()
                    {
                        Title = "Амортизация активной части основных средств",
                        CodeItem = "021"
                    },
                    new()
                    {
                        Title = "Стоимость чистых активов",
                        CodeItem = "100"
                    }
                }
            },
            ReportTypeName = "Бухгалтерский баланс и приложения к нему",
            Footer = new Footer()
        };
    }

    public class Attachment
    {
        public string Text { get; set; }
        public string Title { get; set; }
    }
}