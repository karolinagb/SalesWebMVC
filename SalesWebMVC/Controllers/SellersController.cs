﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using SalesWebMVC.Services.Exceptions;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {
        /*Dependência para o SellerService:*/
        private readonly SellerService _sellerService;

        /*Criando dependência com o DepartmentService:*/
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            //Recebendo a lista de vendedores:
            var list = _sellerService.FindAll();
            //Imprimindo:
            return View(list);
        }

        //IActionResult é o tipo de retorno de todas as ações:
        public IActionResult Create()
        {
            var departments = _departmentService.FindAll();

            var viewModel = new SellerFormViewModel { Departments = departments};

            return View(viewModel);
        }

        //Anotation:
        [HttpPost]
        //Previnindo que aplicação sofra ataque csrf = alguém envia dados maliciosos aproveitando a sua sessao
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            _sellerService.Insert(seller);

            /*Redirect retorna para a ação que eu quiser.
             O nameoof ajuda quando eu mudar o nome da ação, pois ai eu não preciso mudar aq tb*/
            return RedirectToAction(nameof(Index));
        }


        //GET
        //int? = opcional
        public IActionResult Delete(int? id)
        {
            //Se não for digitado o id:
            if(id == null)
            {
                //NotFound instância uma resposta básica:
                return NotFound();
            }

            //Trazer o objeto que estou querendo deletar:
            //Tem que por o id.Value para pegar o valor dele, caso exista, porque ele é um objeto opcional:
            var obj = _sellerService.FindById(id.Value);

            //Se o id não existir:
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);


            return RedirectToAction(nameof(Index));
        }

        //GET
        public IActionResult Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var obj = _sellerService.FindById(id.Value);

            if(obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        /*Esse opcional é só para evitar de acontecer algum erro de execução porque na verdade esse id é obrigatório:
         Por isso testamos se o id é igual a nulo:*/
        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var obj = _sellerService.FindById(id.Value);
            if (obj == null)
            {
                return NotFound();
            }

            //Tenho que recarregar a lista de departamentos para editar o vendedor:
            List<Department> departments = _departmentService.FindAll();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            /*O id do vendedor que eu estou atualizando não pode ser diferente do id da url da requisição*/
            if(id != seller.Id)
            {
                return BadRequest();
            }

            try
            {
                /*Como a operação Update pode gerar exceções vamos colocar isso dentro do try:*/
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (DbConcurrencyException)
            {
                return BadRequest();
            }
            
        }
    }
}
