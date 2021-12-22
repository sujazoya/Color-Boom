using CinemaDirector.Helpers;
using CinemaSuite.Common;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace CinemaDirector
{
    /// <summary>
    /// An Event for enabling any behaviour that has an "enabled" property.
    /// </summary>
    [CutsceneItemAttribute("Game", "Event", CutsceneItemGenre.ActorItem)]
    public class CutsceneActionEvent : CinemaActorEvent
    {
        // The Component/Behaviour to Enable.
   
        public List<EventDelegate> onTime = new List<EventDelegate>();
        public UnityEvent onTimeUnity = new UnityEvent();
        /// <summary>
        /// Trigger this event and send the message.
        /// </summary>
        /// <param name="actor">the actor to send the message to.</param>
        public override void Trigger(GameObject actor)
        {
            if (Application.isPlaying == false) return;

            if (actor != null)
            {
                EventDelegate.Execute(onTime);
                onTimeUnity.Invoke();
            }
        }

    }
}