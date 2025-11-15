using Gresst.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [Route("api/materiales")]
    public class MaterialesController : ControllerBase
    {
        public class Precio
        {
            public long IdMaterial { get; set; }
            public DateTime FechaInicio { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public decimal? PrecioCompra { get; set; }
            public decimal? PrecioServicio { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class PreciosMateriales 
        {
            public string IdGestor { get; set; }
            public string IdCliente { get; set; }
            public string IdDeposito { get; set; }
            public DateTime FechaInicio { get; set; }
            public List<Precio> Precios { get; set; }
        }

        private readonly IMaterialService _materialService;

        public MaterialesController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("get")]
        [Authorize]
        public async Task<ActionResult> Get(CancellationToken cancellationToken, string? filter = null, int pageNumber = 1, int pageSize = 100)
        {
            var materials = await _materialService.GetAllAsync(cancellationToken);

            materials = materials.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            foreach (var material in materials)
                material.Name = material.Name + "-" + material.Description + "-" + material.Measurement;
            return Ok(materials);

        }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    /// <exception cref="HttpResponseException"></exception>
    //    [HttpGet]
    //    [Route("get/{id}")]
    //    public IHttpActionResult Get(int id)
    //    {
    //        UserContext usuario;
    //        string token = Request.Headers.Authorization.Parameter;
    //        var userName = TokenGenerator.ValidateTokenJwt(token);

    //        usuario = AuthenticationService.GetDatosContextoUsuario(userName);
    //        if (usuario is null)
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        try
    //        {
    //            return Ok(MaterialesRepository.List(usuario.UserId, usuario.PersonId).Select(x => new { x.IdEmbalaje, x.Nombre }).FirstOrDefault(x => x.IdEmbalaje == id));
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //            return InternalServerError(ex.GetBaseException());
    //        }
    //    }


    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="filter"></param>
    //    /// <param name="pageNumber"></param>
    //    /// <param name="pageSize"></param>
    //    /// <returns></returns>
    //    /// <exception cref="HttpResponseException"></exception>
    //    [HttpGet]
    //    [Route("getforapp")]
    //    public IHttpActionResult GetForApp(string filter = null, int pageNumber = 1, int pageSize = 100)
    //    {
    //        UserContext usuario;
    //        string token = Request.Headers.Authorization.Parameter;
    //        var userName = TokenGenerator.ValidateTokenJwt(token);

    //        usuario = AuthenticationService.GetDatosContextoUsuario(userName);
    //        if (usuario is null)
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        try
    //        {
    //            var materialesApp = new List<AppMaterial>();
    //            var materiales = MaterialesRepository.List(usuario.UserId, usuario.PersonId);
    //            foreach (Material material in materiales)
    //            {
    //                string tipoCaptura;
    //                string tipoMedicion;
    //                decimal? factor;
    //                if (material.Medicion == Mediciones.Cantidad && material.Peso != null && material.Peso != 0)
    //                {
    //                    tipoCaptura = Mediciones.Peso;
    //                    tipoMedicion = Mediciones.Cantidad;
    //                    factor = material.Peso;
    //                }
    //                else
    //                {
    //                    tipoMedicion = material.Medicion;
    //                    tipoCaptura = material.Medicion;
    //                    factor = 1;
    //                }

    //                var apiMaterial = new AppMaterial()
    //                {
    //                    IdMaterial = material.IdMaterial.ToString(),
    //                    TipoCaptura = tipoCaptura,
    //                    TipoMedicion = tipoMedicion,
    //                    Nombre = material.Nombre,
    //                    Aprovechable = material.Aprovechable,
    //                    PrecioCompra = material.PrecioCompra,
    //                    PrecioServicio = material.PrecioServicio,
    //                    Factor = factor
    //                };
    //                materialesApp.Add(apiMaterial);
    //            }
    //            if (filter != null)
    //                materialesApp = materialesApp.Where(x => x.Nombre.Contains(filter)).ToList();
    //            materialesApp = materialesApp.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //            return Ok(materialesApp);

    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //            return InternalServerError(ex.GetBaseException());
    //        }
    //    }

    //    /// <summary>
    //    /// Trae los materiales (residuos) de un cliente/proveedor del gestor
    //    /// </summary>
    //    /// <param name="idPropietario"></param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("getbyowner/{idPropietario}")]
    //    public HttpResponseMessage GetByOwner(string idPropietario)
    //    {
    //        string jsonResult;
    //        UserContext usuario;
    //        List<MaterialBase> materiales = null;

    //        try
    //        {
    //            usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //            if (usuario is null)
    //                throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //            materiales = MaterialesRepository.GetMateriales(usuario.UserId, idPropietario);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(materiales);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(jsonResult)
    //        };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Trae los datos de un material de un cliente/proveedor del gestor
    //    /// </summary>
    //    /// <param name="idPropietario"></param>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("getbyowner/{idPropietario}/{id}")]
    //    public HttpResponseMessage GetByOwner(string idPropietario, int id)
    //    {
    //        string jsonResult;
    //        UserContext usuario;
    //        List<MaterialBase> materiales = null;

    //        try
    //        {
    //            usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //            if (usuario is null)
    //                throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //            materiales = MaterialesRepository.GetMateriales(usuario.UserId, idPropietario, null, id);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(materiales);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(jsonResult)
    //        };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Trae los materiales de un sitio de recoleccion de un cliente del gestor
    //    /// </summary>
    //    /// <param name="idPropietario"></param>
    //    /// <param name="idDeposito"></param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("getbycollectionsite/{idPropietario}/{idDeposito}")]
    //    public HttpResponseMessage GetByCollectionSite(string idPropietario, long idDeposito)
    //    {
    //        string jsonResult;
    //        UserContext usuario;
    //        List<MaterialBase> materiales = null;

    //        try
    //        {
    //            usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //            if (usuario is null)
    //                throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //            materiales = MaterialesRepository.GetMateriales(usuario.UserId, idPropietario, idDeposito);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(materiales);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(jsonResult)
    //        };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Trae los materiales de un sitio de recoleccion de un cliente del gestor
    //    /// </summary>
    //    /// <param name="idPropietario"></param>
    //    /// <param name="idDeposito"></param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("getbycollectionsite/{idPropietario}/{idDeposito}/{id}")]
    //    public HttpResponseMessage GetByCollectionSite(string idPropietario, long idDeposito, long id)
    //    {
    //        string jsonResult;
    //        UserContext usuario;
    //        List<MaterialBase> materiales = null;

    //        try
    //        {
    //            usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //            if (usuario is null)
    //                throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //            materiales = MaterialesRepository.GetMateriales(usuario.UserId, idPropietario, idDeposito, id);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(materiales);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(jsonResult)
    //        };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Crea o actualiza los datos de materiales para el gestor y para los clientes que estan en la lista
    //    /// </summary>
    //    /// <param name="materiales"></param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [Route("store")]
    //    public HttpResponseMessage Store([FromBody] List<MaterialBase> materiales)
    //    {
    //        UserContext usuario;
    //        List<Response> responses = new List<Response>();
    //        Tratamiento tratamientoBase;
    //        string jsonResult;

    //        usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //        if (usuario is null)
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        if (materiales == null)
    //            throw new HttpResponseException(HttpStatusCode.BadRequest);

    //        foreach (MaterialBase material in materiales)
    //        {
    //            try
    //            {
    //                #region Tipo de residuo
    //                if (material.TipoResiduo != null)
    //                {
    //                    MaterialesRepository.StoreTipoResiduo(usuario.UserId, usuario.AccountId, usuario.PersonId, material.TipoDeResiduo);
    //                    responses.Add(new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, material.TipoDeResiduo.IdTipoResiduo.ToString(), "TipoResiduo", material.TipoDeResiduo.Nombre));
    //                }
    //                else
    //                {
    //                    responses.Add(new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.TipoResiduoNoAlmacenado, "TipoResiduo", material.TipoDeResiduo.Nombre));
    //                    throw new Exception(Resources.Errores.TipoResiduoRequerido);
    //                }
    //                #endregion

    //                #region Embalaje
    //                if (material.Embalaje != null)
    //                {
    //                    try
    //                    {
    //                        EmbalajeRepository.Store(usuario.UserId, usuario.AccountId, usuario.PersonId, material.Embalaje);
    //                        responses.Add(new Response(ResponseStatus.Ok, System.Reflection.MethodBase.GetCurrentMethod().Name, material.Embalaje.IdEmbalaje.ToString(), "Embalaje", material.Embalaje.Nombre));
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        logger.Error(ex.GetBaseException().Message);
    //                        responses.Add(new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.EmbalajeNoAlmacenado, "Embalaje", material.Embalaje.Nombre));
    //                    }
    //                }
    //                #endregion

    //                #region Tratamientos
    //                if (material.Tratamientos != null)
    //                {
    //                    foreach (Tratamiento tratamiento in material.Tratamientos)
    //                    {
    //                        #region Agrega el tratamiento
    //                        if (tratamiento.IdTratamiento != 0)
    //                        {
    //                            tratamientoBase = TratamientoRepository.List(usuario.UserId, usuario.PersonId, tratamiento.IdTratamiento).FirstOrDefault();
    //                        }
    //                        else
    //                        {
    //                            tratamientoBase = TratamientoRepository.GetTratamientoByNombre(usuario.UserId, usuario.PersonId, tratamiento.Nombre);
    //                        }
    //                        if (tratamientoBase != null)
    //                        {
    //                            try
    //                            {
    //                                tratamiento.IdTratamiento = tratamientoBase.IdTratamiento;
    //                                if (material.TipoResiduo.IdTipoResiduo != 0 && tratamiento.IdTratamiento != 0)
    //                                {
    //                                    TratamientoRepository.AddTratamientoTipoResiduo(usuario.UserId, (long)tratamiento.IdTratamiento, (int)material.TipoResiduo.IdTipoResiduo);
    //                                    if (material.IdPropietario != null && material.IdPropietario != "" && material.IdPropietario != usuario.PersonId)
    //                                        TratamientoRepository.AddTratamientoMaterial(usuario.UserId, usuario.AccountId, material.IdMaterial,  (long)tratamiento.IdTratamiento, material.IdPropietario);
    //                                }

    //                            }
    //                            catch (Exception ex)
    //                            {
    //                                logger.Error(ex.Message);
    //                            }
    //                        }
    //                        else
    //                        {
    //                            responses.Add(new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.TratamientoNoExiste, "Tratamiento", tratamiento.Nombre));
    //                        }
    //                        #endregion
    //                    }
    //                }
    //                #endregion

    //                #region Material
    //                MaterialesRepository.StoreMaterial(usuario.UserId, usuario.AccountId, usuario.PersonId, material);
    //                responses.Add(new Response(ResponseStatus.Ok, System.Reflection.MethodBase.GetCurrentMethod().Name, material.IdMaterial.ToString(), "Material", material.Nombre));
    //                #endregion
    //            }
    //            catch (Exception ex)
    //            {
    //                logger.Error(ex.GetBaseException().Message);
    //                responses.Add(new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.MaterialNoAlmacenado, "Material", material.Nombre));
    //            }
    //        }
    //        jsonResult = JsonConvert.SerializeObject(responses);
    //        logger.Debug(jsonResult);
    //        var resp = new HttpResponseMessage() { Content = new StringContent(jsonResult) };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Crea un material para el gestor y los clientes asociados
    //    /// </summary>
    //    /// <param name="material"></param>
    //    /// <returns></returns>
    //    public HttpResponseMessage Post([FromBody] MaterialBase material)
    //    {
    //        UserContext usuario;
    //        Response response = new Response();
    //        string jsonResult;

    //        usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //        if (usuario is null)
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        if (material == null)
    //            throw new HttpResponseException(HttpStatusCode.BadRequest);

    //        try
    //        {
    //            #region Tipo de residuo
    //            if (material.TipoResiduo == null)
    //            {
    //                throw new Exception(Resources.Errores.TipoResiduoRequerido);
    //            }
    //            #endregion

    //            #region Material
    //            MaterialesRepository.CreateMaterial(usuario.UserId, usuario.AccountId, usuario.PersonId, material);
    //            response = new Response(ResponseStatus.Ok, System.Reflection.MethodBase.GetCurrentMethod().Name, material.IdMaterial.ToString(), "Material", material.Nombre);
    //            #endregion
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //            response = new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.MaterialNoCreado, "Material", material.Nombre);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(response);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage() { Content = new StringContent(jsonResult) };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Modifica los datos de un material para el gestor y los clientes asociados
    //    /// </summary>
    //    /// <param name="material"></param>
    //    /// <returns></returns>
    //    public HttpResponseMessage Put([FromBody] MaterialBase material)
    //    {
    //        UserContext usuario;
    //        Response response = new Response();
    //        string jsonResult;

    //        usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //        if (usuario is null)
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        if (material == null)
    //            throw new HttpResponseException(HttpStatusCode.BadRequest);

    //        try
    //        {
    //            #region Tipo de residuo
    //            if (material.TipoResiduo == null)
    //            {
    //                throw new Exception(Resources.Errores.TipoResiduoRequerido);
    //            }
    //            #endregion

    //            #region Material
    //            MaterialesRepository.StoreMaterial(usuario.UserId, usuario.AccountId, usuario.PersonId, material);
    //            response = new Response(ResponseStatus.Ok, System.Reflection.MethodBase.GetCurrentMethod().Name, material.IdMaterial.ToString(), "Material", material.Nombre);
    //            #endregion
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //            response = new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.MaterialNoActualizado, "Material", material.Nombre);
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(response);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage() { Content = new StringContent(jsonResult) };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }

    //    /// <summary>
    //    /// Borra el material id
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    public HttpResponseMessage Delete(long id)
    //    {
    //        UserContext usuario;
    //        Response response = new Response();
    //        string jsonResult;

    //        usuario = AuthenticationService.GetDatosContextoUsuario(User.Identity.Name);
    //        if (usuario is null )
    //            throw new HttpResponseException(HttpStatusCode.Unauthorized);

    //        try
    //        {
    //            MaterialesRepository.DeleteMaterial(usuario.UserId, usuario.AccountId, usuario.PersonId, id);
    //            response = new Response(ResponseStatus.Ok, System.Reflection.MethodBase.GetCurrentMethod().Name, id.ToString(), "Material", id.ToString());
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //            response = new Response(ResponseStatus.Error, System.Reflection.MethodBase.GetCurrentMethod().Name, Resources.Errores.MaterialNoCreado, "Material", id.ToString());
    //        }
    //        finally
    //        {
    //            jsonResult = JsonConvert.SerializeObject(response);
    //            logger.Debug(jsonResult);
    //        }
    //        var resp = new HttpResponseMessage()
    //        {
    //            Content = new StringContent(jsonResult)
    //        };
    //        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        return resp;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="lista"></param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [Route("setbycollectionsite")]
    //    public IHttpActionResult SetByCollectionSite([FromBody] PreciosMateriales lista)
    //    {
    //        UserContext usuario;
    //        string token = Request.Headers.Authorization.Parameter;
    //        var userName = TokenGenerator.ValidateTokenJwt(token);

    //        if (userName is null)
    //            return Unauthorized();

    //        usuario = AuthenticationService.GetDatosContextoUsuario(userName);
    //        if (usuario is null)
    //        {
    //            usuario = AuthenticationService.GetDatosContextoSinUsuario(userName);
    //            if (usuario == null)
    //                return Unauthorized();
    //        }

    //        try
    //        {
    //            foreach (var item in lista.Precios)
    //            {
    //                var material = new Persona_Material { IdCuenta = usuario.AccountId, Activo = true, IdMaterial = item.IdMaterial, IdPersona = lista.IdCliente, PrecioCompra = Tools.ToDecimalNullable(item.PrecioCompra), PrecioServicio = Tools.ToDecimalNullable(item.PrecioServicio) };
    //                MaterialesRepository.SetMaterial(usuario.UserId, material);

    //                if (lista.IdDeposito == null)
    //                {
    //                    var precio = new Persona_Material_Precio { IdCuenta = usuario.AccountId, Activo = true, FechaInicio = Tools.ToDateTime(lista.FechaInicio), IdMaterial = item.IdMaterial, IdPersona = lista.IdCliente, PrecioCompra = Tools.ToDecimalNullable(item.PrecioCompra), PrecioServicio = Tools.ToDecimalNullable(item.PrecioServicio) };
    //                    MaterialesRepository.SetMaterialPrecio(usuario.UserId, precio);
    //                }
    //                else
    //                {
    //                    var materialDeposito = new Persona_Material_Deposito { IdCuenta = usuario.AccountId, Activo = true, IdMaterial = item.IdMaterial, IdPersona = lista.IdCliente, IdDeposito = Tools.ToInt(lista.IdDeposito), PrecioCompra = Tools.ToDecimalNullable(item.PrecioCompra), PrecioServicio = Tools.ToDecimalNullable(item.PrecioServicio) };
    //                    MaterialesRepository.SetMaterialDeposito(usuario.UserId, materialDeposito);

    //                    var precio = new Persona_Material_Deposito_Precio { IdCuenta = usuario.AccountId, Activo = true, FechaInicio = Tools.ToDateTime(lista.FechaInicio), IdMaterial = item.IdMaterial, IdPersona = lista.IdCliente, PrecioCompra = Tools.ToDecimalNullable(item.PrecioCompra), PrecioServicio = Tools.ToDecimalNullable(item.PrecioServicio), IdDeposito = Tools.ToInt(lista.IdDeposito) };
    //                    MaterialesRepository.SetMaterialDepositoPrecio(usuario.UserId, precio);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.GetBaseException().Message);
    //        }
    //        return Ok();
    //    }
    }
}
