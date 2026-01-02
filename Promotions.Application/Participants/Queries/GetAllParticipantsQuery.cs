using MediatR;
using Promotions.Application.Participants.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.Participant.Queries
{
    public class GetAllParticipantsQuery : IRequest<List<ParticipantDto>>
    {
    }
}
