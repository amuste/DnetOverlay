using System;
using System.Collections.Generic;
using System.Linq;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetOverlayComponent.Infrastructure.Services
{
   
    public class OverlayService : IOverlayService
    {
        public event Action<RenderFragment, OverlayConfig> OnAttach;

        public event Action<OverlayResult> OnDetach;

        public event Action OnBackdropClicked;

        private List<OverlayReference> _overlayReferences { get; set; } = new List<OverlayReference>();

        private int _sequenceNumber { get; set; } = 0;

        public OverlayReference GetOverlayReference()
        {
            _sequenceNumber++;

            var overlayReference = new OverlayReference(_sequenceNumber);

            _overlayReferences.Add(overlayReference);

            return overlayReference;
        }

        public OverlayReference Attach(RenderFragment overlayContent, OverlayConfig overlayConfig)
        {
            _sequenceNumber++;

            var overlayReference = new OverlayReference(_sequenceNumber);

            _overlayReferences.Add(overlayReference);

            overlayConfig.OverlayRef = overlayReference.OverlayReferenceId;

            OnAttach?.Invoke(overlayContent, overlayConfig);

            return overlayReference;
        }

        public void Detach(OverlayResult overlayDataResult)
        {
            var item = _overlayReferences.Find(p => p.OverlayReferenceId == overlayDataResult.OverlayRef);

            if(item == null) return;

            _overlayReferences.Remove(item);

            if (!_overlayReferences.Any()) _sequenceNumber = 0;

            OnDetach?.Invoke(overlayDataResult);

            item.CloseOverlayReference(overlayDataResult);
        }

        public void BackdropClicked(OverlayResult overlayDataResult)
        {
            Detach(overlayDataResult);
        }
    }
}
