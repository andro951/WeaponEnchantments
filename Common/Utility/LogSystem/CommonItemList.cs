using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
    public class CommonItemList
    {
        public List<Item> CommonList { get; private set; }
        public List<Item> UniqueList { get; private set; }
        public List<Item> All => UniqueList != null ? CommonList.Concat(UniqueList).ToList() : CommonList;
        public int Count => (CommonList != null ? CommonList.Count : 0) + (UniqueList != null ? 1 : 0);
        public string CommonToAll {
            get {
                if (commonToAll == null) {
                    commonToAll = UniqueList.Select(i => i.ToItemPNG()).ToList().CommonToAll();
                }

                return commonToAll;
            }
        }
        private string commonToAll;
        public CommonItemList(List<Item> list, Item uniqueItem = null) {
            if (uniqueItem != null) {
                UniqueList = new() { uniqueItem };
            }

            CommonList = new(list);
        }
        public static bool FindUniqueItems(List<Item> list, List<Item> toCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            toCheckUnique = null;
            listUnique = null;
            listIndex = -1;
            List<int> listTypes = list.Select(i => i.type).ToList();
            List<int> toCheckStrings = toCheck.Select(i => i.type).ToList();
            int listCount = list.Count;
            bool[] listFoundInToCheck = new bool[listCount];
            for (int i = 0; i < toCheckStrings.Count; i++) {
                int toCheckType = toCheckStrings[i];
                bool match = false;
                for (int k = 0; k < listCount; k++) {
                    int listType = listTypes[k];
                    if (toCheckType == listType) {
                        match = true;
                        listFoundInToCheck[k] = true;
                        break;
                    }
                }

                if (!match) {
                    if (toCheckUnique != null) {
                        return false;
                    }
                    else {
                        toCheckUnique = toCheck[i];
                    }
                }
            }

            //if (toCheckUnique == null)
            //    return false;

            for (int i = 0; i < listCount; i++) {
                if (!listFoundInToCheck[i]) {
                    if (listUnique != null) {
                        return false;
                    }
                    else {
                        listUnique = list[i];
                        listIndex = i;
                    }
                }
            }

            return true;
        }
        public static bool FindUniqueItems(CommonItemList list, List<Item> toCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            List<Item> common = list.UniqueList == null ? list.CommonList : list.CommonList.Concat(new Item[] { list.UniqueList[0] }).ToList();
            //return FindUniqueItems(common, toCheck, out listUnique, out toCheckUnique, out listIndex);
            return FindUniqueItems(common, toCheck, out listUnique, out toCheckUnique, out listIndex);
        }
        public static bool FindUniqueItems(CommonItemList list, CommonItemList listToCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            if (listToCheck.UniqueList == null)
                return FindUniqueItems(list, listToCheck.CommonList, out listUnique, out toCheckUnique, out listIndex);

            listUnique = null;
            toCheckUnique = null;
            listIndex = -1;

            return false;
        }
        public static bool UniqueItemTypesMatch(Item item, Item other) {
            Type listUniqueType = item.TypeAboveModItem();
            Type toCheckUniqueType = other.TypeAboveModItem();

            return listUniqueType == toCheckUniqueType;
        }
        public static bool UniqueItemTypesMatch(List<Item> list, List<Item> toCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            if (!FindUniqueItems(list, toCheck, out listUnique, out toCheckUnique, out listIndex))
                return false;

            return UniqueItemTypesMatch(listUnique, toCheckUnique);
        }
        public static bool UniqueItemTypesMatch(CommonItemList list, List<Item> toCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            if (!FindUniqueItems(list, toCheck, out listUnique, out toCheckUnique, out listIndex))
                return false;

            return UniqueItemTypesMatch(listUnique, toCheckUnique);
        }
        public static bool UniqueItemTypesMatch(CommonItemList list, CommonItemList listToCheck, out Item listUnique, out Item toCheckUnique, out int listIndex) {
            if (!FindUniqueItems(list, listToCheck, out listUnique, out toCheckUnique, out listIndex))
                return false;

            return UniqueItemTypesMatch(listUnique, toCheckUnique);
        }
        public bool Add(List<Item> list) {
            CommonItemList newList = new(list);

            return Add(newList);
        }
        public bool Add(CommonItemList other) {
            if (other.UniqueList == null) {
                if (!UniqueItemTypesMatch(this, other, out Item myUnique, out Item otherUnique, out int myCommonIndex))
                    return false;

                if (myCommonIndex != -1 && UniqueList == null) {
                    CommonList.RemoveAt(myCommonIndex);
                    UniqueList = new() { myUnique };
                }

                if (otherUnique != null && !UniqueList.Select(i => i.type).Contains(otherUnique.type))
                    UniqueList.Add(otherUnique);

                return true;
            }

            if (UniqueList == null)
                return false;

            UniqueList.CombineLists(other.UniqueList, true);

            return true;
        }
        public bool TryAdd(CommonItemList other) {
            if (SameCommonItemsCompareTypesIgnoreStack(other))
                return Add(other);

            return false;
        }
        public bool SameCommonItems(CommonItemList other) {
            bool nullUniqueList = other.UniqueList == null;
            List<string> myCommonList = CommonList.Select(i => i.ToString()).ToList();
            List<string> otherCommonList = other.CommonList.Select(i => i.ToString()).ToList();
            if (Count != other.Count)
                return false;

            int failedMatchCount = 0;
            foreach (string s in otherCommonList) {
                if (!myCommonList.Contains(s)) {
                    failedMatchCount++;
                    if (failedMatchCount > 0 && !nullUniqueList || failedMatchCount > 1 && nullUniqueList)
                        return false;
                }
            }

            return true;
        }
        public bool SameCommonItemsCompareTypesIgnoreStack(CommonItemList other) {
            bool nullUniqueList = other.UniqueList == null;
            if (Count != other.Count)
                return false;

            List<int> myCommonList = CommonList.Select(i => i.type).ToList();
            List<int> otherCommonList = other.CommonList.Select(i => i.type).ToList();
            int failedMatchCount = 0;
            for (int i = 0; i < other.CommonList.Count; i++) {
                int num = otherCommonList[i];
                if (!myCommonList.Contains(num)) {
                    failedMatchCount++;
                    if (failedMatchCount >= 0 && !nullUniqueList || failedMatchCount > 1 && nullUniqueList)
                        return false;
                }
            }

            return true;
        }
        public int NumberCommonItems(CommonItemList other) {
            if (CommonList.Count == 0 || other.CommonList.Count == 0)
                return 0;

            bool nullUniqueList = other.UniqueList == null;
            List<string> myCommonList = CommonList.Select(i => i.ToString()).ToList();
            List<string> otherCommonList = other.CommonList.Select(i => i.ToString()).ToList();

            int failedMatchCount = 0;
            foreach (string s in otherCommonList) {
                if (!myCommonList.Contains(s)) {
                    failedMatchCount++;
                }
            }

            return CommonList.Count - failedMatchCount;
        }
        public bool ExactSame(CommonItemList other) => UniqueList.SameAs(other.UniqueList) && CommonList.SameAs(other.CommonList);
        public bool Contains(Item item) => CommonList.Select(i => i.type).Contains(item.type) || UniqueList != null && UniqueList.Select(i => i.type).Contains(item.type);
        public override string ToString() {
            string text = "";
            if (CommonList.Count > 0) {
                foreach (string s in CommonList.Select(i => i.ToItemPNG(link: true))) {
                    text += $"{s}<br/>";
                }
            }

            if (UniqueList == null)
                return text;

            int uniqueCount = UniqueList.Count;
            if (uniqueCount >= 20) {
                text += CommonToAll;
            }
            else if (uniqueCount > 0) {
                if (text != "")
                    text += "and<br/>";

                bool first = true;
                foreach (string s in UniqueList.Select(i => i.ToItemPNG(link: true))) {
                    if (first) {
                        first = false;
                    }
                    else {
                        text += $"<br/>or<br/>";
                    }

                    text += $"{s}";
                }
            }

            return text;
        }
    }
}
