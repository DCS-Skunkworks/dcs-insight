﻿using System;
using System.Diagnostics;
using System.Text;
using DCSInsight.JSON;

namespace DCSInsight.Misc
{
    internal class ResultComparator
    {
        private readonly DCSAPI _dcsApi;
        private readonly object _lockObject = new();

        public ResultComparator(DCSAPI dcsApi)
        {
            _dcsApi = dcsApi;
            _dcsApi.Result = int.MinValue.ToString(); // Set so that first value will be listed in the results.
        }

        public bool IsMatch(DCSAPI dcsApi)
        {
            try
            {
                lock (_lockObject)
                {
                    if (dcsApi.Parameters.Count != _dcsApi.Parameters.Count) return false;

                    for (var i = 0; i < _dcsApi.ParamCount; i++)
                    {
                        if (_dcsApi.Parameters[i].Value != dcsApi.Parameters[i].Value)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }

            return true;
        }

        /// <summary>
        /// Returns true if result is different since previous check
        /// </summary>
        /// <param name="dcsApi"></param>
        /// <returns></returns>
        public bool SetResult(DCSAPI dcsApi)
        {
            try
            {
                lock (_lockObject)
                {
                    if (!IsMatch(dcsApi))
                    {
                        throw new Exception("SetResult() : This is not the matching DCSAPI.");
                    }

                    if (dcsApi.Result != _dcsApi.Result)
                    {
                        _dcsApi.Result = dcsApi.Result;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }

            return false;
        }

        public string GetResultString()
        {
            lock (_lockObject)
            {
                var currentTestString = new StringBuilder();

                foreach (var dcsApiParameter in _dcsApi.Parameters)
                {
                    currentTestString.Append($"{dcsApiParameter.ParameterName} [{dcsApiParameter.Value}], ");
                }

                currentTestString.Append($" result : {_dcsApi.Result}\n");

                return currentTestString.ToString();
            }
        }
    }
}
