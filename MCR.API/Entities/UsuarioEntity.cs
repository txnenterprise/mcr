using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;



namespace MCR.API.Entities
{
    public class UsuarioEntity : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string LoginProvider { get; set; }
        public string Document { get; set; }




        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        [NotMapped]
        public string Message { get; set; }
        [NotMapped]
        public bool Success { get; set; }

        public virtual ICollection<CotacoesAgricolaEntity> CotacoesAgricola { get; set; } = new List<CotacoesAgricolaEntity>();

        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; } = new List<IdentityUserClaim<Guid>>();

        public virtual ICollection<PropostasEntity> Propostas { get; set; } = new List<PropostasEntity>();
        public virtual ICollection<UsuarioCanalEntity> UsuarioCanal { get; set; } = new List<UsuarioCanalEntity>();
        public virtual ICollection<UsuarioCanalEntity> UsuarioCanalLiderado { get; set; } = new List<UsuarioCanalEntity>();

        public virtual ICollection<UsuarioPontoAtendimentoEntity> UsuarioPontoAtendimento { get; set; } = new List<UsuarioPontoAtendimentoEntity>();
        public virtual ICollection<UsuarioPontoAtendimentoEntity> UsuarioPontoAtendimentoLiderado { get; set; } = new List<UsuarioPontoAtendimentoEntity>();
        public override Guid Id { get => base.Id; set => base.Id = value; }
        public virtual ICollection<PropostasObservacoesEntity> PropostasObservacoes { get; set; } = new List<PropostasObservacoesEntity>();

        public virtual ICollection<PropostasDocumentosEntity> PropostasDocumentos { get; set; } = new List<PropostasDocumentosEntity>();

        public virtual ICollection<UsuarioEstruturaNegocioEntity> UsuarioEstruturaNegocio { get; set; } = new List<UsuarioEstruturaNegocioEntity>();
        public virtual ICollection<UsuarioEstruturaNegocioEntity> UsuarioEstruturaNegocioCriador { get; set; } = new List<UsuarioEstruturaNegocioEntity>();
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }
        public override bool TwoFactorEnabled { get => base.TwoFactorEnabled; set => base.TwoFactorEnabled = value; }
        public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }
        public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }
    }
}
