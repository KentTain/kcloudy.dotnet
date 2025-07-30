using System.Text;
using KC.Framework.Exceptions;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KC.Service
{
    public static class ServiceWrapper
    {
        public static ServiceResult<T> Invoke<T>(string serviceName, string methodName,
            Func<T> function, ILogger logger)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            T result;
            string message = string.Empty;
            var detailsMsg = "调用服务({0})的方法({1})操作{2}。";
            try
            {
                result = function();
                //message = string.Format(detailsMsg, serviceName, methodName, "成功");
                //logger.LogInfo(message);
                return new ServiceResult<T>(ServiceResultType.Success, "操作成功！", result);
            }
            catch (ArgumentNullException argnullex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + argnullex.Message);
                logger.LogError(argnullex, detailsMsg);
                message = argnullex.Message;
                return new ServiceResult<T>(ServiceResultType.QueryNull, message);
            }
            catch (ArgumentException argnullex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + argnullex.Message);
                logger.LogError(argnullex, detailsMsg);
                message = argnullex.Message;
                return new ServiceResult<T>(ServiceResultType.ParamError, message);
            }
            catch (ComponentException cex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + cex.Message);
                logger.LogError(cex, detailsMsg);
                message = cex.Message;
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            catch (DataAccessException daex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + daex.Message);
                logger.LogError(daex, detailsMsg);
                message = daex.Message;
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            catch (BusinessException bex)
            {
                detailsMsg = bex.Message;
                logger.LogError(bex, string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bex.Message));
                return new ServiceResult<T>(ServiceResultType.Error, detailsMsg);
            }
            catch (BusinessPromptException bpx)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bpx.Message);
                logger.LogError(bpx, detailsMsg);
                return new ServiceResult<T>(ServiceResultType.Error, bpx.Message);
            }
            catch (BusinessApiException bpx)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bpx.Message);
                logger.LogError(bpx, detailsMsg);
                return new ServiceResult<T>(ServiceResultType.Error, bpx.Message);
            }
            catch (Exception ex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + ex.Message);
                logger.LogError(ex, detailsMsg);
                message = ex.Message;

                if (ex.InnerException == null)
                    return new ServiceResult<T>(ServiceResultType.Error, message);

                //if (ex.InnerException is SqlException)
                //{
                //    var sqlex = ex.InnerException as SqlException;
                //    var sberrors = new StringBuilder();
                //    foreach (SqlError err in sqlex.Errors)
                //    {
                //        string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))
                //            ? DataHelper.GetSqlExceptionMessage(sqlex.Number)
                //            : err.Message;
                //        sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                //    }

                //    message = sberrors.Length > 0 ? sberrors.ToString() : sqlex.Message;

                //    logger.LogError(sqlex, message, "堆栈消息为：" + sqlex.StackTrace);
                //}
                //else
                {
                    logger.LogError(ex.InnerException, ex.InnerException.Message);
                }
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            finally
            {
                watch.Stop();
                logger.LogDebug(string.Format("{0}：执行服务（{1}）时间总共为: {2}",
                    serviceName, methodName, watch.ElapsedMilliseconds));
            }
        }

        public static async Task<ServiceResult<T>> InvokeAsync<T>(string serviceName, string methodName,
            Func<Task<T>> function, ILogger logger)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            T result;
            string message = string.Empty;
            var detailsMsg = "调用服务({0})的方法({1})操作{2}。";
            try
            {
                result = await function();
                //message = string.Format(detailsMsg, serviceName, methodName, "成功");
                //LogUtil.LogInfo(message);
                return new ServiceResult<T>(ServiceResultType.Success, "操作成功！", result);
            }
            catch (ArgumentNullException argnullex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + argnullex.Message);
                logger.LogError(argnullex, detailsMsg);
                message = argnullex.Message;
                return new ServiceResult<T>(ServiceResultType.QueryNull, message);
            }
            catch (ArgumentException argnullex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + argnullex.Message);
                logger.LogError(argnullex, detailsMsg);
                message = argnullex.Message;
                return new ServiceResult<T>(ServiceResultType.ParamError, message);
            }
            catch (ComponentException cex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + cex.Message);
                logger.LogError(cex, detailsMsg);
                message = cex.Message;
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            catch (DataAccessException daex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + daex.Message);
                logger.LogError(daex, detailsMsg);
                message = daex.Message;
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            catch (BusinessException bex)
            {
                detailsMsg = bex.Message;
                logger.LogError(bex, string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bex.Message));
                return new ServiceResult<T>(ServiceResultType.Error, detailsMsg);
            }
            catch (BusinessPromptException bpx)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bpx.Message);
                logger.LogError(bpx, detailsMsg);
                return new ServiceResult<T>(ServiceResultType.Error, bpx.Message);
            }
            catch (BusinessApiException bpx)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + bpx.Message);
                logger.LogError(bpx, detailsMsg);
                return new ServiceResult<T>(ServiceResultType.Error, bpx.Message);
            }
            catch (Exception ex)
            {
                detailsMsg = string.Format(detailsMsg, serviceName, methodName, "失败，错误消息为：" + ex.Message);
                logger.LogError(ex, detailsMsg);
                message = ex.Message;
                if (ex.InnerException == null)
                    return new ServiceResult<T>(ServiceResultType.Error, message);

                //if (ex.InnerException is SqlException)
                //{
                //    var sqlex = ex.InnerException as SqlException;
                //    var sberrors = new StringBuilder();
                //    foreach (SqlError err in sqlex.Errors)
                //    {
                //        string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))
                //            ? DataHelper.GetSqlExceptionMessage(sqlex.Number)
                //            : err.Message;
                //        sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                //    }

                //    message = sberrors.Length > 0 ? sberrors.ToString() : sqlex.Message;

                //    logger.LogError(sqlex, message, "堆栈消息为：" + sqlex.StackTrace);
                //}
                //else
                {
                    logger.LogError(ex.InnerException, ex.InnerException.Message);
                }
                return new ServiceResult<T>(ServiceResultType.Error, message);
            }
            finally
            {
                watch.Stop();
                logger.LogDebug(string.Format("{0}：执行服务（{1}）时间总共为: {2}",
                    serviceName, methodName, watch.ElapsedMilliseconds));
            }
        }
    }
}
