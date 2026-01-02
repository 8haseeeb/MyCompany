using MediatR;
using Promotions.Application.Participants.Dtos;
using System.Collections.Generic;

public record GetParticipantsByActionQuery(int IdAction) : IRequest<List<ParticipantDto>>;
