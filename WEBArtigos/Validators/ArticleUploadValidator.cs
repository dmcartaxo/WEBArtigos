using FluentValidation;
using WEBArtigos.DTOs;

namespace WEBArtigos.Validators;

public class ArticleUploadValidator : AbstractValidator<ArticleUploadDto>
{
    public ArticleUploadValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("Arquivo PDF é obrigatório.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .Length(3, 300).WithMessage("Título deve ter entre 3 e 300 caracteres.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Autor é obrigatório.")
            .Length(2, 150).WithMessage("Autor deve ter entre 2 e 150 caracteres.");
    }
}
