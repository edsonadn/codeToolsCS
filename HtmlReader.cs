public class ListHtml
    {
        public readonly List<Parameter> parameters = new List<Parameter>();
        public readonly List<ListHtml> children = new List<ListHtml>();
        public string nameTag { get; set; }
        public string valueAfter { get; set; }
        public string valueBefore { get; set; }
        public ListHtml(string path, string _nameTag)
        {
            nameTag = _nameTag;
            string text = File.ReadAllText(path).Replace("\r\n", string.Empty);

            List<string> listString = text.Split('<').ToList();
            List<string[]> htmlList = new List<string[]>();

            listString.ForEach((val) =>
            {
                htmlList.Add(val.Split('>'));
            });
            GenerateInitialLimits(htmlList);
        }
        public ListHtml(List<string[]> htmlList, string _nameTag)
        {
            nameTag = _nameTag;
            GenerateInitialLimits(htmlList);
        }
        private void GenerateInitialLimits(List<string[]> htmlList)
        {
            if (comprobeTag(nameTag))
            {

                int breaker1 = -1, breaker2 = -1;
                List<string[]> listChildren = new List<string[]>();

                for (int i = 0; i < htmlList.Count; i++)
                {
                    if (htmlList[i][0].StartsWith(nameTag))
                    {
                        breaker1 = i;
                        CreateParameters(htmlList[breaker1][0]);
                        valueAfter = htmlList[breaker1][1];
                        break;
                    }
                }
                int countTag = 0;
                for (int i = breaker1; i < htmlList.Count; i++)
                {
                    if (htmlList[i][0].StartsWith(nameTag))
                    {
                        countTag++;
                    }
                    else if (htmlList[i][0].StartsWith("/" + nameTag) && countTag == 1)
                    {
                        breaker2 = i;
                        valueBefore = htmlList[breaker2][1];
                        break;
                    }
                    else if (htmlList[i][0].StartsWith("/" + nameTag))
                    {
                        countTag--;
                    }
                }
                if (breaker1 != -1 && breaker2 != -1)
                {
                    listChildren = htmlList;
                    int lim1 = breaker2;
                    int lim2 = listChildren.Count - breaker2;
                    listChildren.RemoveRange(lim1, lim2);
                    listChildren.RemoveRange(0, breaker1 + 1);
                    GetChildren(listChildren);
                }
            }
        }
        private void CreateParameters(string tag)
        {
            string[] cutParameter = tag.Split(' ');
            List<string> listParameters = cutParameter.ToList();
            listParameters.ForEach((parameter) =>
            {
                var parameterAndValue = parameter.Split('=');

                if (parameterAndValue.Length > 1)
                {
                    parameters.Add(new Parameter { Key = parameter.Split('=')[0], Value = parameter.Split('=')[1].Replace("\"", string.Empty) });
                }
            });
        }
        private bool comprobeTag(string nameTag)
        {
            if (nameTag.ToLower().Contains("img") ||
                nameTag.ToLower().Contains("br") ||
                nameTag.ToLower().Contains("meta"))
            {
                return false;
            }
            return true;
        }
        private RangeAndList DetectRang(List<string[]> list, string tagHtml, int rangeInit)
        {
            RangeAndList range = new RangeAndList() { Breaker1 = -1, Breaker2 = -1 };

            List<string[]> result = new List<string[]>();

            for (int i = rangeInit; i < list.Count; i++)
            {
                if (list[i][0].StartsWith(tagHtml))
                {
                    range.Breaker1 = i;
                    break;
                }
            }
            if (range.Breaker1 != -1)
            {
                int countTag = 0;
                for (int i = range.Breaker1; i < list.Count; i++)
                {
                    result.Add(list[i]);
                    if (list[i][0].StartsWith(tagHtml))
                    {
                        countTag++;
                    }
                    else if (list[i][0].StartsWith("/" + tagHtml) && countTag == 1)
                    {
                        range.Breaker2 = i;
                        break;
                    }
                    else if (list[i][0].StartsWith("/" + tagHtml))
                    {
                        countTag--;
                    }
                }
            }
            if (range.Breaker1 != -1 && range.Breaker2 != -1)
            {
                range.Name = tagHtml;
                range.list = result;
                return range;
            }
            return new RangeAndList();

        }
        private void GetChildren(List<string[]> sublist)
        {
            for (int i = 0; i < sublist.Count; i++)
            {
                if (comprobeTag(sublist[i][0].Split(' ')[0]))
                {
                    i += CreateChildren(DetectRang(sublist, sublist[i][0].Split(' ')[0], i));
                }
                else
                {
                    Console.WriteLine("Cancel Children:" + sublist[i][0].Split(' ')[0]);
                }
            }
        }
        private int CreateChildren(RangeAndList rangeAndList)
        {
            if (rangeAndList.list != null && rangeAndList.Name != null)
            {
                children.Add(new ListHtml(rangeAndList.list, rangeAndList.Name));
            }
            return rangeAndList.Breaker2 - rangeAndList.Breaker1;
        }
        public struct Parameter
        {
            public string Key;
            public string Value;
        }
        public struct RangeAndList
        {
            public int Breaker1;
            public int Breaker2;
            public List<string[]> list;
            public string Name;
        }
    }
