﻿using System.ComponentModel.DataAnnotations;

namespace DesapegAutoWeb.Models
{
    /// <summary>
    /// ViewModel para o formulário de cadastro e edição de Pessoa.
    /// </summary>
    public class PessoaViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CPF deve estar no formato 000.000.000-00.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [StringLength(15, MinimumLength = 14, ErrorMessage = "O telefone deve estar no formato (00) 00000-0000.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 50 caracteres.")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmação de Senha")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmacaoSenha { get; set; }
    }
}