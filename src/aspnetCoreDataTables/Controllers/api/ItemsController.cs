using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using aspnetCoreDataTables.Models;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace aspnetCoreDataTables.Controllers.api
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        // POST api/Items
        [HttpPost]
        public IActionResult Items()
        {
            long filteredCount = 0;

            var requestFormData = Request.Form;
            List<Item> itemList = GetData();

            var listItems = ProcessCollection(itemList, requestFormData, out filteredCount);

            dynamic response = new
            {
                Data = listItems,
                Draw = requestFormData["draw"],
                RecordsFiltered = filteredCount,
                RecordsTotal = itemList.Count
            };
            return Ok(response);
        }

        private List<Item> GetData()
        {
            List<Item> itemList = new List<Item>()
            {
                new Item() {ItemId = 33, Name = "Airi Satou",  Description ="Accountant"},
                new Item() {ItemId = 47, Name = "Angelica Ramos",  Description ="Chief Executive Officer (CEO)"},
                new Item() {ItemId = 66, Name = "Ashton Cox",  Description ="Junior Technical Author"},
                new Item() {ItemId = 41, Name = "Bradley Greer",  Description ="Software Engineer"},
                new Item() {ItemId = 28, Name = "Brenden Wagner",  Description ="Software Engineer"},
                new Item() {ItemId = 61, Name = "Brielle Williamson",  Description ="Integration Specialist"},
                new Item() {ItemId = 38, Name = "Bruno Nash",  Description ="Software Engineer"},
                new Item() {ItemId = 21, Name = "Caesar Vance",  Description ="Pre-Sales Support"},
                new Item() {ItemId = 46, Name = "Cara Stevens",  Description ="Sales Assistant"},
                new Item() {ItemId = 22, Name = "Cedric Kelly",  Description ="Senior Javascript Developer"},
                new Item() {ItemId = 36, Name = "Charde Marshall",  Description ="Regional Director"}
            };

            var length = 10;
            for (int i = 0; i < length; i++)
            {
                itemList.AddRange(itemList);
            }
            return itemList;
        }

        private PropertyInfo getProperty(string name)
        {
            var properties = typeof(Models.Item).GetProperties();
            PropertyInfo prop = null;
            foreach (var item in properties)
            {
                if (item.Name.ToLower().Equals(name.ToLower()))
                {
                    prop = item;
                    break;
                }
            }
            return prop;
        }

        private List<Models.Item> ProcessCollection(List<Models.Item> lstElements, IFormCollection requestFormData, out long count)
        {
            count = 0;
            var skip = Convert.ToInt32(requestFormData["start"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());

            Microsoft.Extensions.Primitives.StringValues tempOrder = new[] { "" };
            if (requestFormData.TryGetValue("order[0][column]", out tempOrder))
            {
                var columnIndex = requestFormData["order[0][column]"].ToString();
                var sortDirection = requestFormData["order[0][dir]"].ToString();
                tempOrder = new[] { "" };
                if (requestFormData.TryGetValue($"columns[{columnIndex}][data]", out tempOrder))
                {
                    var columName = requestFormData[$"columns[{columnIndex}][data]"].ToString();

                    if (pageSize > 0)
                    {
                        var prop = getProperty(columName);
                        if (sortDirection == "asc")
                        {
                            Microsoft.Extensions.Primitives.StringValues tempSearch = new[] { "" };
                            if (requestFormData.TryGetValue("search[value]", out tempSearch))
                            {
                                var searchValue = requestFormData["search[value]"].ToString();

                                if (String.IsNullOrEmpty(searchValue))
                                {
                                    var list = lstElements.OrderBy(prop.GetValue).Skip(skip).Take(pageSize).ToList();

                                    count = lstElements.Count;

                                    return list;
                                }
                                else
                                {
                                    var query = lstElements.OrderBy(prop.GetValue).Where(w => Regex.IsMatch(w.Name, searchValue, RegexOptions.IgnoreCase) || Regex.IsMatch(w.Description, searchValue, RegexOptions.IgnoreCase));

                                    count = query.Count();

                                    return query.Skip(skip).Take(pageSize).ToList();
                                }


                            }

                        }
                        else
                        {
                            Microsoft.Extensions.Primitives.StringValues tempSearch = new[] { "" };
                            if (requestFormData.TryGetValue("search[value]", out tempSearch))
                            {
                                var searchValue = requestFormData["search[value]"].ToString();

                                if (String.IsNullOrEmpty(searchValue))
                                {
                                    var list = lstElements.OrderByDescending(prop.GetValue).Skip(skip).Take(pageSize).ToList();

                                    count = lstElements.Count;

                                    return list;

                                } else
                                {
                                    var query = lstElements.OrderByDescending(prop.GetValue).Where(w => Regex.IsMatch(w.Name, searchValue, RegexOptions.IgnoreCase) || Regex.IsMatch(w.Description, searchValue, RegexOptions.IgnoreCase));

                                    count = query.Count();

                                    return query.Skip(skip).Take(pageSize).ToList();
                                }


                            }

                        }
                    }
                    else
                    {
                        count = lstElements.Count;

                        return lstElements;
                    }
                }
            }
            return null;
        }
    }
}
